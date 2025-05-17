import React, { useState } from 'react';
import { updateUserPhone } from '../services/userService';
import { useError } from '../contexts/ErrorContext';
import { ErrorType } from '../hooks/useErrorHandler';

interface PhoneVerificationProps {
  userId: string;
  currentPhone: string;
  onSuccess: (newPhoneNumber: string) => void;
}

// Константы типов ошибок для телефона
const PHONE_ERRORS = {
  EMPTY_PHONE: ErrorType.EmptyPhoneNumberError460,
  INVALID_FORMAT: ErrorType.InvalidPhoneFormatError461,
  USER_NOT_FOUND: ErrorType.UserDataNotFoundError201,
  UPDATE_FAILED: ErrorType.UserUpdateFailedError202,
  PERMISSION_DENIED: ErrorType.UserPermissionDeniedError203,
};

const PhoneVerificationComponent: React.FC<PhoneVerificationProps> = ({ 
  userId, 
  currentPhone, 
  onSuccess 
}) => {
  const [showForm, setShowForm] = useState(false);
  const [newPhone, setNewPhone] = useState('');
  const [loading, setLoading] = useState(false);
  const { showError } = useError();

  // Проверяем, совпадает ли номер телефона с +79999999999
  const shouldVerify = currentPhone === '+79999999999';

  // Если номер не требует проверки, то ничего не показываем
  if (!shouldVerify) return null;

  const handleVerified = () => {
    setShowForm(false);
    onSuccess(currentPhone);
  };

  const handleUpdatePhone = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!newPhone) {
      showError('Пожалуйста, введите новый номер телефона', 'warning');
      return;
    }
    
    // Форматируем телефон в формат +7XXXXXXXXXX
    const phoneDigits = newPhone.replace(/\D/g, '');
    
    // Проверяем формат номера телефона
    if (!/^7\d{10}$/.test(phoneDigits) && !/^\d{10}$/.test(phoneDigits)) {
      showError('Номер телефона должен содержать 10 цифр после кода +7', 'warning');
      return;
    }
    
    // Форматируем телефон в нужный формат
    let formattedPhone = phoneDigits;
    if (formattedPhone.length === 10) {
      formattedPhone = `+7${formattedPhone}`;
    } else if (formattedPhone.length === 11 && formattedPhone.startsWith('7')) {
      formattedPhone = `+${formattedPhone}`;
    } else {
      showError('Неверный формат номера телефона', 'warning');
      return;
    }
    
    setLoading(true);
    
    try {
      await updateUserPhone(userId, formattedPhone);
      showError('Номер телефона успешно обновлен!', 'success');
      onSuccess(formattedPhone);
    } catch (error: any) {
      // Проверяем код ошибки из ответа сервера
      const errorCode = error?.response?.data?.errorCode || error?.response?.data?.errorType;
      let errorMessage = 'Ошибка при обновлении номера телефона';
      
      // Выбираем сообщение об ошибке в зависимости от кода
      if (errorCode === PHONE_ERRORS.EMPTY_PHONE) {
        errorMessage = 'Номер телефона не может быть пустым';
      } else if (errorCode === PHONE_ERRORS.INVALID_FORMAT) {
        errorMessage = 'Неверный формат номера телефона. Формат должен быть: +7XXXXXXXXXX';
      } else if (errorCode === PHONE_ERRORS.USER_NOT_FOUND) {
        errorMessage = 'Пользователь не найден';
      } else if (errorCode === PHONE_ERRORS.UPDATE_FAILED) {
        errorMessage = 'Ошибка при обновлении номера телефона';
      } else if (errorCode === PHONE_ERRORS.PERMISSION_DENIED) {
        errorMessage = 'У вас нет прав на обновление этого номера телефона';
      } else {
        // Если код неизвестен, пытаемся получить сообщение из ответа
        errorMessage = error.response?.data?.message || 
                       error.message || 
                       'Ошибка при обновлении номера телефона';
      }
      
      showError(errorMessage, 'warning');
    } finally {
      setLoading(false);
    }
  };

  // Проверка правильности формата номера телефона
  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // Разрешаем +, цифры и скобки
    const sanitizedValue = value.replace(/[^\d+() -]/g, '');
    setNewPhone(sanitizedValue);
  };

  return (
    <div className="mb-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
      {!showForm ? (
        <div>
          <h3 className="text-lg font-medium text-yellow-700 mb-2">Проверка номера телефона</h3>
          <p className="mb-4 text-yellow-600">
            В вашем профиле указан номер телефона: <strong>{currentPhone}</strong> <br/>
            Это ваш действующий номер телефона?
          </p>
          <div className="flex space-x-3">
            <button
              onClick={handleVerified}
              className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600 transition-colors"
            >
              Да, это мой номер
            </button>
            <button
              onClick={() => setShowForm(true)}
              className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600 transition-colors"
            >
              Нет, нужно обновить
            </button>
          </div>
        </div>
      ) : (
        <div>
          <h3 className="text-lg font-medium text-yellow-700 mb-2">Обновление номера телефона</h3>
          <form onSubmit={handleUpdatePhone} className="space-y-4">
            <div>
              <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                Введите ваш текущий номер телефона:
              </label>
              <input
                type="tel"
                id="phone"
                name="phone"
                placeholder="+7 (XXX) XXX-XX-XX"
                value={newPhone}
                onChange={handlePhoneChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                disabled={loading}
              />
              <p className="mt-1 text-sm text-gray-500">Формат: +7XXXXXXXXXX (например: +79991234567)</p>
            </div>
            <div className="flex space-x-3">
              <button
                type="submit"
                className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition-colors flex items-center"
                disabled={loading}
              >
                {loading && (
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                )}
                Сохранить
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="px-4 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400 transition-colors"
                disabled={loading}
              >
                Отмена
              </button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
};

export default PhoneVerificationComponent; 