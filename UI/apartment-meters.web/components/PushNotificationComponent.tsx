import React, { useEffect, useState } from 'react';
import { usePushNotification } from '../hooks/usePushNotification';

/**
 * Компонент для запроса разрешения на уведомления
 * Показывает разные сообщения в зависимости от статуса разрешений
 */
const PushNotificationComponent: React.FC = () => {
  const { autoSubscribe } = usePushNotification();
  const [permissionStatus, setPermissionStatus] = useState<NotificationPermission | 'default'>('default');
  
  // Проверяем разрешение напрямую
  useEffect(() => {
    const checkPermission = () => {
      if ('Notification' in window) {
        setPermissionStatus(Notification.permission);
      }
    };
    
    // Проверяем при загрузке
    checkPermission();
    
    // Проверяем при возврате на вкладку
    const handleVisibilityChange = () => {
      if (document.visibilityState === 'visible') {
        checkPermission();
      }
    };
    
    document.addEventListener('visibilitychange', handleVisibilityChange);
    
    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange);
    };
  }, []);

  // Функция для запроса разрешения
  const handleRequestPermission = async () => {
    if (!('Notification' in window)) return;
    
    try {
      const permission = await Notification.requestPermission();
      setPermissionStatus(permission);
      
      if (permission === 'granted') {
        await autoSubscribe();
      }
    } catch (error) {
      console.error('Ошибка при запросе разрешения:', error);
    }
  };

  // Не показываем компонент, если уведомления уже разрешены
  if (permissionStatus === 'granted') {
    return null;
  }

  // Показываем инструкции, если уведомления заблокированы
  if (permissionStatus === 'denied') {
    return (
      <div className="bg-gradient-to-r from-red-50 to-orange-50 border border-red-100 shadow-sm p-3 rounded-lg">
        <div className="flex items-center space-x-3 mb-2">
          <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-red-500" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
          <span className="font-medium text-gray-700">Уведомления заблокированы в настройках браузера</span>
        </div>
        <div className="pl-8 text-sm text-gray-600">
          <p>Чтобы получать важные оповещения, разрешите уведомления в настройках браузера:</p>
          <ol className="list-decimal ml-5 mt-1">
            <li>Нажмите на иконку 🔒 в адресной строке</li>
            <li>Выберите "Настройки сайта" или "Разрешения"</li>
            <li>Найдите раздел "Уведомления" и измените значение на "Разрешить"</li>
          </ol>
        </div>
      </div>
    );
  }

  // Стандартный запрос разрешения
  return (
    <div className="bg-gradient-to-r from-blue-50 to-indigo-50 border border-blue-100 shadow-sm p-3 rounded-lg flex justify-between items-center">
      <div className="flex items-center space-x-3">
        <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-blue-500" viewBox="0 0 20 20" fill="currentColor">
          <path d="M10 2a6 6 0 00-6 6v3.586l-.707.707A1 1 0 004 14h12a1 1 0 00.707-1.707L16 11.586V8a6 6 0 00-6-6zM10 18a3 3 0 01-3-3h6a3 3 0 01-3 3z" />
        </svg>
        <span className="font-medium text-gray-700">Включите уведомления для получения важных сообщений</span>
      </div>
      <button 
        className="bg-blue-500 hover:bg-blue-600 transition-colors text-white font-medium text-sm py-2 px-4 rounded-md shadow-sm"
        onClick={handleRequestPermission}
      >
        Разрешить
      </button>
    </div>
  );
};

export default PushNotificationComponent;