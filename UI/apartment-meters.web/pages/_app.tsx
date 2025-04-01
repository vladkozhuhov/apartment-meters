import '../styles/globals.css';
import type { AppProps } from 'next/app';
import { useState, useEffect } from 'react';
import { ErrorProvider } from '../contexts/ErrorContext';
import ErrorAlert from '../components/ErrorAlert';
import dynamic from 'next/dynamic';
import api from '../services/api';
import { setErrorHandler } from '../services/api';
import pushNotificationService from '../services/pushNotificationService';

// Расширяем интерфейс Console для добавления нашего свойства
declare global {
  interface Console {
    _errorOriginal?: typeof console.error;
    _restored?: boolean;
  }
  
  interface Window {
    _originalOnError?: typeof window.onerror;
    _originalUnhandledRejection?: OnUnhandledRejectionEventHandler;
  }
}

type OnUnhandledRejectionEventHandler = (this: Window, ev: PromiseRejectionEvent) => any;

/**
 * Глобальный обработчик ошибок для всего приложения
 */
if (typeof window !== 'undefined') {
  // Подавляем ошибки addStyle, которые возникают из-за React DevTools
  const originalError = window.onerror;
  window.onerror = function(message, source, lineno, colno, error) {
    if (message && (
      String(message).includes('addStyle') || 
      String(message).includes('port closed') ||
      String(message).includes('ResizeObserver')
    )) {
      return true; // Предотвращаем дальнейшую обработку ошибки
    }
    
    if (originalError) {
      return originalError.apply(window, [message, source, lineno, colno, error] as any);
    }
    return false;
  };
}

// Динамический импорт компонента Layout
const Layout = dynamic(() => import('./layout'), { ssr: false });

/**
 * Инициализация обработчика ошибок API и глобальных обработчиков ошибок
 */
const ErrorHandlerSetup: React.FC = () => {
  useEffect(() => {
    // Устанавливаем обработчик ошибок API
    const handleApiError = (message: string, severity?: 'error' | 'warning' | 'info' | 'success') => {
      // Мы не обрабатываем ошибки здесь, так как это делается через ErrorContext
      // Эта функция вызывается из API перехватчика
    };
    
    // Регистрируем обработчик в API
    setErrorHandler(handleApiError);

    // Устанавливаем глобальные перехватчики ошибок для предотвращения краша приложения
    if (typeof window !== 'undefined') {
      // Сохраняем оригинальные функции консоли
      const originalConsoleError = console.error;
      
      // Перехватываем ошибки React при рендеринге
      console.error = function(...args: any[]) {
        // Фильтруем известные безвредные ошибки
        const errorString = args.join(' ');
        
        // Игнорируем ошибки API 400, которые уже обрабатываются компонентами
        if (
          (errorString.includes('Request failed with status code 400') || 
           errorString.includes('AxiosError: Request failed with status code 400'))
        ) {
          return; // Полностью игнорируем ошибки API с кодом 400
        }
        
        if (
          errorString.includes('addStyle') ||
          errorString.includes('port closed') ||
          errorString.includes('ResizeObserver') ||
          errorString.includes('форсированное обновление')
        ) {
          // Не выводим в консоль
          return;
        }
        
        // Используем оригинальный метод для других ошибок
        originalConsoleError.apply(console, args);
      };
      
      // Перехватываем непойманные ошибки промисов
      const handleUnhandledRejection = (event: any) => {
        try {
          // Предотвращаем показ ошибки 400 в интерфейсе Next.js
          const reason = event.reason || event.error || event.detail?.reason || {};
          console.log('Перехвачено unhandledrejection событие:', reason);
          
          // Проверка на ошибки Axios 400 (Bad Request)
          const isAxios400Error = 
            reason.message?.includes('Request failed with status code 400') || 
            (reason.isAxiosError && reason.response?.status === 400) ||
            event.type === 'unhandledrejection' && reason.name === 'AxiosError' && reason.response?.status === 400;
          
          if (isAxios400Error) {
            console.log('ПЕРЕХВАЧЕНА ошибка Axios 400 (обработана):', reason.config?.url || 'неизвестный URL');
            event.preventDefault?.();
            event.stopPropagation?.();
            event.stopImmediatePropagation?.();
            event.cancelBubble = true;
            return true; // Отметка, что ошибка обработана
          }
        } catch (e) {
          console.error('Ошибка в обработчике unhandledrejection:', e);
        }
        return false; // Пусть другие обработчики работают
      };
      
      // Используем различные способы подписки для максимальной совместимости
      window.addEventListener('unhandledrejection', handleUnhandledRejection, { capture: true });
      window.onunhandledrejection = handleUnhandledRejection;
    }

    // Очищаем слушатели при размонтировании
    return () => {
      if (typeof window !== 'undefined') {
        // Восстанавливать все обработчики не нужно,
        // так как компонент не будет размонтирован до закрытия страницы
      }
    };
  }, []);
  
  return null;
};

function MyApp({ Component, pageProps }: AppProps) {
  const [isClient, setIsClient] = useState(false);

  useEffect(() => {
    setIsClient(true);
    
    // Регистрируем сервис-воркер для PWA и автоматически подписываем на уведомления
    if (
      typeof window !== 'undefined' && 
      'serviceWorker' in navigator
    ) {
      try {
        // Проверяем поддержку и пытаемся зарегистрировать сервис-воркер
        if (pushNotificationService.isSupported()) {
          // Регистрируем сервис-воркер
          pushNotificationService.registerServiceWorker()
            .then(() => {
              console.log('Сервис-воркер успешно зарегистрирован');
              
              // Проверяем, не подписан ли уже пользователь
              return pushNotificationService.getSubscriptionStatus();
            })
            .then((status) => {
              // Если пользователь еще не подписан, пытаемся подписать
              if (status === 'not-subscribed') {
                return pushNotificationService.subscribe();
              }
            })
            .then((subscription) => {
              if (subscription) {
                console.log('Пользователь автоматически подписан на уведомления');
              }
            })
            .catch((error) => {
              console.error('Ошибка при работе с сервис-воркером:', error);
            });
        }
      } catch (error) {
        console.error('Ошибка при инициализации сервис-воркера:', error);
      }
    }
  }, []);

  if (!isClient) {
    return null;
  }

  return (
    <ErrorProvider>
      <ErrorHandlerSetup />
      
      {/* Основной контент */}
      <Layout>
        <Component {...pageProps} />
      </Layout>
      
      {/* Глобальный компонент для отображения ошибок */}
      <ErrorAlert />
    </ErrorProvider>
  );
}

export default MyApp;