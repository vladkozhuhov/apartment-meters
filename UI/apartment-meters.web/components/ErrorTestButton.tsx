import React from 'react';
import { useError } from '../contexts/ErrorContext';
import useErrorHandler from '../hooks/useErrorHandler';
import api from '../services/api';

/**
 * Компонент для тестирования системы отображения ошибок
 * Отображается только в режиме разработки
 */
const ErrorTestButton: React.FC = () => {
  // Если не режим разработки, не отображаем компонент
  if (process.env.NODE_ENV !== 'development') {
    return null;
  }
  
  const { showError } = useError();
  const { handleError } = useErrorHandler();

  const showErrorHandler = (severity: 'error' | 'warning' | 'info' | 'success') => {
    switch (severity) {
      case 'error':
        showError('Тест ошибки: Критическая ошибка в приложении', 'error');
        break;
      case 'warning':
        showError('Тест предупреждения: Пожалуйста, обратите внимание на важную информацию', 'warning');
        break;
      case 'info':
        showError('Тест информации: Ваши данные успешно обновлены', 'info');
        break;
      case 'success':
        showError('Тест успеха: Операция выполнена успешно', 'success');
        break;
    }
  };

  const simulateApiError = async () => {
    // Создаем имитацию ошибки API для демонстрации обработки ошибок
    const mockError = {
      response: {
        status: 400,
        data: {
          detail: 'Тестовая ошибка валидации: Обязательные поля не заполнены',
          errors: {
            username: ['Имя пользователя обязательно'],
            password: ['Пароль должен быть не менее 8 символов']
          }
        }
      }
    };

    // Вызываем наш обработчик ошибок
    handleError(mockError);
  };

  const testDirectApiError = () => {
    // Напрямую используем функцию из API для отображения ошибки
    api.__showErrorMessage('Тест ошибки от API: Сервер временно недоступен', 'error');
  };

  return (
    <div className="flex flex-col space-y-2 p-4 border rounded-lg border-gray-300 bg-gray-50">
      <h3 className="text-lg font-medium mb-2">Тестирование уведомлений (только для разработки)</h3>
      <div className="flex flex-wrap gap-2">
        <button
          onClick={() => showErrorHandler('error')}
          className="px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600"
        >
          Ошибка
        </button>
        <button
          onClick={() => showErrorHandler('warning')}
          className="px-3 py-1 bg-yellow-500 text-white rounded hover:bg-yellow-600"
        >
          Предупреждение
        </button>
        <button
          onClick={() => showErrorHandler('info')}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600"
        >
          Информация
        </button>
        <button
          onClick={() => showErrorHandler('success')}
          className="px-3 py-1 bg-green-500 text-white rounded hover:bg-green-600"
        >
          Успех
        </button>
        <button
          onClick={simulateApiError}
          className="px-3 py-1 bg-purple-500 text-white rounded hover:bg-purple-600"
        >
          Имитация API ошибки
        </button>
        <button
          onClick={testDirectApiError}
          className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-800"
        >
          Прямая ошибка API
        </button>
      </div>
    </div>
  );
};

export default ErrorTestButton; 