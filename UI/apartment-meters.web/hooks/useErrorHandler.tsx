import { useCallback } from 'react';
import { useError } from '../contexts/ErrorContext';

/**
 * Типы ошибок, соответствующие ErrorType из бэкенда (ErrorMessages.cs)
 */
export enum ErrorType {
  // Ошибки авторизации (100-199)
  UserNotFoundError101 = 'UserNotFoundError101',
  InvalidPasswordError102 = 'InvalidPasswordError102',
  UserBlockedError103 = 'UserBlockedError103',
  InvalidTokenError104 = 'InvalidTokenError104',
  
  // Ошибки пользователей (200-299)
  UserDataNotFoundError201 = 'UserDataNotFoundError201',
  UserUpdateFailedError202 = 'UserUpdateFailedError202',
  UserPermissionDeniedError203 = 'UserPermissionDeniedError203',
  
  // Ошибки квартир и счетчиков (300-399)
  ApartmentDataNotFoundError301 = 'ApartmentDataNotFoundError301',
  UserDeletionFailedError302 = 'UserDeletionFailedError302',
  WaterMeterNotFoundError351 = 'WaterMeterNotFoundError351',
  MeterReadingNotFoundError352 = 'MeterReadingNotFoundError352',
  MeterReadingLessThanPreviousError353 = 'MeterReadingLessThanPreviousError353',
  InvalidMeterReadingFormatError354 = 'InvalidMeterReadingFormatError354',
  
  // Ошибки валидации данных (400-499)
  InvalidDataFormatError401 = 'InvalidDataFormatError401',
  MissingRequiredParametersError402 = 'MissingRequiredParametersError402',
  
  // Валидация пользователей (450-469)
  EmptyApartmentNumberError450 = 'EmptyApartmentNumberError450',
  InvalidApartmentNumberError451 = 'InvalidApartmentNumberError451',
  EmptyLastNameError452 = 'EmptyLastNameError452',
  LastNameTooLongError453 = 'LastNameTooLongError453',
  EmptyFirstNameError454 = 'EmptyFirstNameError454',
  FirstNameTooLongError455 = 'FirstNameTooLongError455',
  MiddleNameTooLongError456 = 'MiddleNameTooLongError456',
  EmptyPasswordError457 = 'EmptyPasswordError457',
  PasswordTooShortError458 = 'PasswordTooShortError458',
  InvalidPasswordFormatError459 = 'InvalidPasswordFormatError459',
  EmptyPhoneNumberError460 = 'EmptyPhoneNumberError460',
  InvalidPhoneFormatError461 = 'InvalidPhoneFormatError461',
  InvalidUserRoleError462 = 'InvalidUserRoleError462',
  EmptyUserIdError463 = 'EmptyUserIdError463',
  
  // Валидация счетчиков (470-479)
  EmptyWaterMeterIdError470 = 'EmptyWaterMeterIdError470',
  InvalidWaterMeterPlaceError471 = 'InvalidWaterMeterPlaceError471',
  InvalidWaterTypeError472 = 'InvalidWaterTypeError472',
  EmptyFactoryNumberError473 = 'EmptyFactoryNumberError473',
  FactoryNumberTooLongError474 = 'FactoryNumberTooLongError474',
  InvalidFactoryNumberFormatError475 = 'InvalidFactoryNumberFormatError475',
  EmptyFactoryYearError476 = 'EmptyFactoryYearError476',
  FutureFactoryYearError477 = 'FutureFactoryYearError477',
  
  // Валидация показаний (480-489)
  EmptyWaterValueError480 = 'EmptyWaterValueError480',
  InvalidWaterValueFormatError481 = 'InvalidWaterValueFormatError481',
  EmptyReadingDateError482 = 'EmptyReadingDateError482',
  FutureReadingDateError483 = 'FutureReadingDateError483',
  MeterReadingOutsideAllowedPeriodError484 = 'MeterReadingOutsideAllowedPeriodError484',
  
  // Системные ошибки (500+)
  InternalServerError501 = 'InternalServerError501',
  ServiceUnavailableError502 = 'ServiceUnavailableError502'
}

/**
 * Хук для профессиональной обработки ошибок API
 * Предоставляет функции для извлечения сообщений из различных форматов ошибок
 * и отображения пользовательских уведомлений
 */
export const useErrorHandler = () => {
  const { showError, clearError } = useError();

  /**
   * Извлекает структурированное сообщение об ошибке из различных форматов ответов API
   * @param error - Объект ошибки или ответ API
   */
  const extractErrorMessage = useCallback((error: any): string => {
    // Нет ошибки
    if (!error) return 'Неизвестная ошибка';

    // Приоритет - оригинальное сообщение с бэкенда
    if (error.response?.data) {
      const data = error.response.data;
      
      // Используем оригинальное сообщение в порядке приоритета
      if (typeof data.message === 'string') return data.message;
      if (typeof data.detail === 'string') return data.detail;
      if (typeof data.title === 'string') return data.title;
      if (typeof data.Message === 'string') return data.Message;
      
      // Для ошибок валидации собираем все в одну строку
      if (data.errors) {
        const errorsList = data.errors;
        const messages = Object.values(errorsList)
          .flat()
          .filter(Boolean)
          .join(', ');
        if (messages) return messages;
      }
    }
    
    // Стандартное сообщение JavaScript
    if (error.message) {
      return error.message;
    }
    
    return 'Произошла ошибка при выполнении запроса';
  }, []);

  /**
   * Возвращает текстовое сообщение для кода ошибки из ErrorMessages.cs
   */
  const getErrorMessageByCode = (errorCode: string): string | null => {
    switch (errorCode) {
      // Ошибки авторизации
      case ErrorType.UserNotFoundError101:
        return 'Пользователь с таким номером квартиры не найден.';
      case ErrorType.InvalidPasswordError102:
        return 'Неправильный пароль.';
      case ErrorType.UserBlockedError103:
        return 'Пользователь заблокирован.';
      case ErrorType.InvalidTokenError104:
        return 'Токен авторизации недействителен.';
      
      // Ошибки пользователей
      case ErrorType.UserDataNotFoundError201:
        return 'Данные пользователя не найдены.';
      case ErrorType.UserUpdateFailedError202:
        return 'Ошибка при обновлении данных пользователя.';
      case ErrorType.UserPermissionDeniedError203:
        return 'У пользователя отсутствуют права на это действие.';
      
      // Ошибки квартир и счетчиков
      case ErrorType.ApartmentDataNotFoundError301:
        return 'Данные квартиры не найдены.';
      case ErrorType.UserDeletionFailedError302:
        return 'Невозможно удалить данные по пользователю.';
      case ErrorType.WaterMeterNotFoundError351:
        return 'Счетчик с указанным идентификатором не найден.';
      case ErrorType.MeterReadingNotFoundError352:
        return 'Показание счетчика с указанным идентификатором не найдено.';
      case ErrorType.MeterReadingLessThanPreviousError353:
        return 'Новое показание не может быть меньше предыдущего.';
      case ErrorType.InvalidMeterReadingFormatError354:
        return 'Неверный формат показания счетчика. Формат должен быть \'целое,дробное\' (до 5 цифр до запятой и до 3 после).';
      
      // Ошибки валидации данных
      case ErrorType.InvalidDataFormatError401:
        return 'Неправильный формат данных.';
      case ErrorType.MissingRequiredParametersError402:
        return 'Отсутствуют обязательные параметры.';
      
      // Валидация пользователей
      case ErrorType.EmptyApartmentNumberError450:
        return 'Номер квартиры не может быть пустым.';
      case ErrorType.InvalidApartmentNumberError451:
        return 'Номер квартиры должен быть положительным числом.';
      case ErrorType.EmptyLastNameError452:
        return 'Фамилия не может быть пустой.';
      case ErrorType.LastNameTooLongError453:
        return 'Фамилия не может быть длиннее 50 символов.';
      case ErrorType.EmptyFirstNameError454:
        return 'Имя не может быть пустым.';
      case ErrorType.FirstNameTooLongError455:
        return 'Имя не может быть длиннее 50 символов.';
      case ErrorType.MiddleNameTooLongError456:
        return 'Отчество не может быть длиннее 50 символов.';
      case ErrorType.EmptyPasswordError457:
        return 'Пароль не может быть пустым.';
      case ErrorType.PasswordTooShortError458:
        return 'Пароль должен содержать минимум 8 символов.';
      case ErrorType.InvalidPasswordFormatError459:
        return 'Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру.';
      case ErrorType.EmptyPhoneNumberError460:
        return 'Номер телефона не может быть пустым.';
      case ErrorType.InvalidPhoneFormatError461:
        return 'Номер телефона должен быть в формате +7XXXXXXXXXX.';
      case ErrorType.InvalidUserRoleError462:
        return 'Указана недопустимая роль пользователя.';
      case ErrorType.EmptyUserIdError463:
        return 'Идентификатор пользователя не может быть пустым.';
      
      // Валидация счетчиков
      case ErrorType.EmptyWaterMeterIdError470:
        return 'Идентификатор счетчика не может быть пустым.';
      case ErrorType.InvalidWaterMeterPlaceError471:
        return 'Указано недопустимое расположение счетчика.';
      case ErrorType.InvalidWaterTypeError472:
        return 'Указан недопустимый тип счетчика.';
      case ErrorType.EmptyFactoryNumberError473:
        return 'Заводской номер счетчика не может быть пустым.';
      case ErrorType.FactoryNumberTooLongError474:
        return 'Заводской номер счетчика не может быть длиннее 10 символов.';
      case ErrorType.InvalidFactoryNumberFormatError475:
        return 'Заводской номер должен содержать только буквы и цифры.';
      case ErrorType.EmptyFactoryYearError476:
        return 'Дата установки счетчика не может быть пустой.';
      case ErrorType.FutureFactoryYearError477:
        return 'Дата установки счетчика не может быть в будущем.';
      
      // Валидация показаний
      case ErrorType.EmptyWaterValueError480:
        return 'Показание счетчика не может быть пустым.';
      case ErrorType.InvalidWaterValueFormatError481:
        return 'Формат показаний должен содержать до 5 цифр до запятой и до 3 после.';
      case ErrorType.EmptyReadingDateError482:
        return 'Дата показания не может быть пустой.';
      case ErrorType.FutureReadingDateError483:
        return 'Дата показания не может быть в будущем.';
      case ErrorType.MeterReadingOutsideAllowedPeriodError484:
        return 'Показания можно подавать только с 23 по 25 число месяца.';
      
      // Системные ошибки
      case ErrorType.InternalServerError501:
        return 'Внутренняя ошибка сервера.';
      case ErrorType.ServiceUnavailableError502:
        return 'Временная недоступность сервиса.';
      
      default:
        return null;
    }
  };

  /**
   * Определяет тип уведомления в зависимости от ошибки
   * @param error - Объект ошибки или ответ API
   */
  const determineSeverity = useCallback((error: any): 'error' | 'warning' | 'info' | 'success' => {
    // Проверяем наличие кода ошибки из ErrorMessages.cs
    if (error.response?.data?.errorCode || error.response?.data?.errorType) {
      const errorCode = error.response.data.errorCode || error.response.data.errorType;
      
      // Определяем тип сообщения по коду ошибки
      if (errorCode) {
        // Ошибки 400-499 - пользовательские, отображаем как warning
        if (errorCode.match(/Error4\d\d$/)) {
          return 'warning';
        }
        
        // Ошибки 500+ - серверные, отображаем как error
        if (errorCode.match(/Error5\d\d$/)) {
          return 'error';
        }
        
        // Ошибки авторизации и валидации - предупреждения
        if (
          errorCode.includes('NotFound') ||
          errorCode.includes('Invalid') ||
          errorCode.includes('Empty') ||
          errorCode.match(/Error1\d\d$/) ||
          errorCode.match(/Error45\d$/) ||
          errorCode.match(/Error46\d$/) ||
          errorCode.match(/Error47\d$/) ||
          errorCode.match(/Error48\d$/)
        ) {
          return 'warning';
        }
      }
    }
    
    // Пользовательские ошибки (400-499) отображаем как предупреждения
    if (error?.response?.status && error.response.status >= 400 && error.response.status < 500) {
      return 'warning';
    }
    
    // Определяем тип по содержимому сообщения
    const message = extractErrorMessage(error).toLowerCase();
    
    // Сообщения о пользовательских ошибках показываем как предупреждения
    if (
      message.includes('не найден') || 
      message.includes('неправильный') || 
      message.includes('неверный') || 
      message.includes('должен быть') || 
      message.includes('требуется') ||
      message.includes('не может быть') ||
      message.includes('уже существует')
    ) {
      return 'warning';
    }
    
    // По умолчанию считаем системной ошибкой
    return 'error';
  }, [extractErrorMessage]);

  /**
   * Обработчик ошибок для отображения пользовательских уведомлений
   * @param error - Объект ошибки или ответ API
   */
  const handleError = useCallback((error: any) => {
    const message = extractErrorMessage(error);
    const severity = determineSeverity(error);
    
    showError(message, severity);
    
    return message;
  }, [extractErrorMessage, determineSeverity, showError]);

  return {
    handleError,
    extractErrorMessage,
    determineSeverity,
    showError,
    clearError
  };
};

export default useErrorHandler; 