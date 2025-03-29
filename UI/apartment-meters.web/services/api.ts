import axios, { AxiosInstance } from 'axios';
import { API_BASE_URL, API_TIMEOUT, API_DEFAULT_HEADERS } from '../config/api';

// Ограничиваем лог для предотвращения спама в консоли
let logCount = 0;
const MAX_LOGS = 20; // Уменьшаем количество логов

// Расширяем тип AxiosInstance для поддержки пользовательских свойств
interface ExtendedAxiosInstance extends AxiosInstance {
  __showErrorMessage: (message: string, severity?: 'error' | 'warning' | 'info' | 'success') => void;
}

// Функция логирования с ограничением
const limitedLog = (type: 'log' | 'error' | 'warn', ...args: any[]) => {
  if (logCount < MAX_LOGS) {
    if (type === 'log') console.log(...args);
    else if (type === 'error') console.error(...args);
    else if (type === 'warn') console.warn(...args);
    logCount++;
    
    if (logCount === MAX_LOGS) {
      console.warn('Достигнут лимит логов API. Дальнейшие логи будут скрыты.');
    }
  }
};

// Создаем экземпляр axios с базовым URL и таймаутом
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
  headers: API_DEFAULT_HEADERS
}) as ExtendedAxiosInstance;

// Функция для обработки ошибок - будет установлена позже
let errorHandler = (message: string, severity?: 'error' | 'warning' | 'info' | 'success') => {
  // Отключаем логи по умолчанию
};

// Функция для установки обработчика ошибок
export const setErrorHandler = (handler: (message: string, severity?: 'error' | 'warning' | 'info' | 'success') => void) => {
  // Отключаем логи
  errorHandler = handler;
};

// Прямая функция показа ошибок
api.__showErrorMessage = (message: string, severity: 'error' | 'warning' | 'info' | 'success' = 'error') => {
  try {
    errorHandler(message, severity);
  } catch (err) {
    // Если не удалось показать ошибку через errorHandler, логируем в консоль
    console.error('[API] Не удалось отобразить ошибку:', message);
  }
};

// Функция для извлечения структурированного сообщения об ошибке из ответа API
const extractStructuredErrorMessage = (error: any): string => {
  // Проверяем наличие структурированного ответа от бэкенда
  if (error?.response?.data) {
    // Проверяем наличие полей сообщений об ошибке в разных форматах
    if (typeof error.response.data.detail === 'string') return error.response.data.detail;
    if (typeof error.response.data.message === 'string') return error.response.data.message;
    if (typeof error.response.data.title === 'string') return error.response.data.title;
    if (typeof error.response.data.Message === 'string') return error.response.data.Message;
    
    // Проверяем наличие массива ошибок валидации (стандарт ASP.NET Core)
    if (error.response.data.errors) {
      const errorsList = error.response.data.errors;
      // Собираем все ошибки в одну строку
      const errorMessages = Object.values(errorsList)
        .flat()
        .filter(Boolean)
        .join(', ');
      
      if (errorMessages) return errorMessages;
    }
  }
  
  // Извлекаем сообщение из самого объекта ошибки
  if (error?.message && !error.message.includes('html') && !error.message.includes('HTML')) {
    return error.message;
  }
  
  // Определяем сообщение на основе HTTP статуса
  if (error?.response?.status) {
    switch (error.response.status) {
      case 400: return 'Неверный запрос или формат данных';
      case 401: return 'Требуется авторизация';
      case 403: return 'Доступ запрещен';
      case 404: return 'Ресурс не найден';
      case 500: return 'Внутренняя ошибка сервера';
    }
  }
  
  // Для HTML-ответов
  if (error?.message && (error.message.includes('html') || error.message.includes('HTML'))) {
    return 'Сервер недоступен или вернул некорректный ответ';
  }
  
  // Возвращаем обобщенное сообщение, если ничего не найдено
  return 'Произошла ошибка при выполнении запроса';
};

// Определяем тип ошибки на основе кода статуса и содержимого сообщения
const determineErrorSeverity = (error: any): 'error' | 'warning' | 'info' | 'success' => {
  // Ошибки валидации и пользовательские ошибки (400-499) считаем предупреждениями
  if (error?.response?.status && error.response.status >= 400 && error.response.status < 500) {
    return 'warning';
  }
  
  // Если сообщение об ошибке указывает на пользовательскую проблему, показываем как предупреждение
  const message = extractStructuredErrorMessage(error).toLowerCase();
  if (
    message.includes('не найден') || 
    message.includes('неправильный') || 
    message.includes('неверный') || 
    message.includes('должен') || 
    message.includes('не может')
  ) {
    return 'warning';
  }
  
  // По умолчанию считаем серверной ошибкой
  return 'error';
};

// Добавляем перехватчик для добавления токена к запросам
api.interceptors.request.use(
  config => {
    // Отключаем логи
    
    const token = localStorage.getItem('token');
    if (token) {
      // Отключаем логи
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    // Отключаем логи
    return Promise.reject(error);
  }
);

// Перехватчик для обработки ответов
api.interceptors.response.use(
  (response) => {
    // Если сервер вернул HTML вместо JSON (например, страница 404)
    const contentType = response.headers['content-type'];
    if (contentType && contentType.includes('text/html')) {
      return Promise.reject(new Error('Сервер вернул HTML-страницу вместо ожидаемого JSON'));
    }
    return response;
  },
  (error) => {
    // Предотвращаем всплытие "Unhandled Runtime Error" для ошибок валидации 400
    if (error.response && error.response.status === 400) {
      // Добавляем флаг, указывающий, что эта ошибка уже обработана
      error.isHandledApiError = true;
      // Добавляем сообщение для пользователя, если нужно отобразить его вне компонента
      error.userMessage = extractStructuredErrorMessage(error);
      error.severity = determineErrorSeverity(error);
      
      // Только логируем, но не показываем на глобальном уровне
      limitedLog('error', 'Обработанная ошибка валидации (400):', error.config.url);
      limitedLog('log', 'Детали ошибки:', error.response.data);
      
      return Promise.reject(error);
    }
    
    // Обработка других ошибок как обычно
    if (error.response) {
      limitedLog('error', 'Ошибка ответа:', error.response.status, error.config.url);
      
      // Для ошибок авторизации не логируем подробности, чтобы не засорять консоль
      if (!error.config.url?.includes('/api/auth/login')) {
        limitedLog('error', 'Детали ошибки:', error.response.data);
      }
      
      if (error.response.status === 401 && !error.config.url?.includes('/api/auth/login')) {
        limitedLog('log', 'Получен статус 401 Unauthorized, перенаправление на /login');
        localStorage.removeItem('token');
        localStorage.removeItem('id');
        localStorage.removeItem('apartmentNumber');
        
        // Перенаправляем на страницу логина, если это не страница логина
        if (typeof window !== 'undefined' && window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }
    } else {
      limitedLog('error', 'Ошибка сети:', error.message);
    }
    return Promise.reject(error);
  }
);

// Проверяет, является ли ответ HTML (например, страница 404 от сервера)
const isHtmlResponse = (responseData: any): boolean => {
  if (typeof responseData === 'string' && 
    (responseData.includes('<!DOCTYPE html>') || 
     responseData.includes('<html>') || 
     responseData.includes('<body>'))) {
    return true;
  }
  return false;
};

export default api;