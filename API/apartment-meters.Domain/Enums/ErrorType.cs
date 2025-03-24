namespace Domain.Enums;

/// <summary>
/// Коды ошибок для API
/// </summary>
public enum ErrorType
{
    #region Ошибки аутентификации и авторизации

    /// <summary>
    /// Пользователь с таким номером квартиры не найден
    /// </summary>
    UserNotFoundError101 = 101,

    /// <summary>
    /// Неправильный пароль
    /// </summary>
    InvalidPasswordError102 = 102,

    /// <summary>
    /// Пользователь заблокирован
    /// </summary>
    UserBlockedError103 = 103,

    /// <summary>
    /// Токен авторизации недействителен
    /// </summary>
    InvalidTokenError104 = 104,

    #endregion

    #region Ошибки, связанные с действиями пользователя

    /// <summary>
    /// Данные пользователя не найдены
    /// </summary>
    UserDataNotFoundError201 = 201,

    /// <summary>
    /// Ошибка при обновлении данных пользователя
    /// </summary>
    UserUpdateFailedError202 = 202,

    /// <summary>
    /// У пользователя отсутствуют права на это действие
    /// </summary>
    UserPermissionDeniedError203 = 203,

    #endregion
    
    #region Ошибки, связанные с действиями администратора

    /// <summary>
    /// Данные квартиры не найдены
    /// </summary>
    ApartmentDataNotFoundError301 = 301,

    /// <summary>
    /// Невозможно удалить данные по пользователю
    /// </summary>
    UserDeletionFailedError302 = 302,

    #endregion

    #region Ошибки, связанные с показаниями счетчиков

    /// <summary>
    /// Счетчик не найден
    /// </summary>
    WaterMeterNotFoundError351 = 351,

    /// <summary>
    /// Показание счетчика не найдено
    /// </summary>
    MeterReadingNotFoundError352 = 352,

    /// <summary>
    /// Новое показание меньше предыдущего
    /// </summary>
    MeterReadingLessThanPreviousError353 = 353,

    /// <summary>
    /// Неверный формат показания счетчика
    /// </summary>
    InvalidMeterReadingFormatError354 = 354,

    #endregion

    #region Ошибки валидации данных

    /// <summary>
    /// Неправильный формат данных
    /// </summary>
    InvalidDataFormatError401 = 401,

    /// <summary>
    /// Отсутствуют обязательные параметры
    /// </summary>
    MissingRequiredParametersError402 = 402,

    #region Ошибки валидации пользователей (450-469)
    
    /// <summary>
    /// Номер квартиры не может быть пустым
    /// </summary>
    EmptyApartmentNumberError450 = 450,
    
    /// <summary>
    /// Номер квартиры должен быть положительным числом
    /// </summary>
    InvalidApartmentNumberError451 = 451,
    
    /// <summary>
    /// Фамилия не может быть пустой
    /// </summary>
    EmptyLastNameError452 = 452,
    
    /// <summary>
    /// Фамилия не может быть длиннее 50 символов
    /// </summary>
    LastNameTooLongError453 = 453,
    
    /// <summary>
    /// Имя не может быть пустым
    /// </summary>
    EmptyFirstNameError454 = 454,
    
    /// <summary>
    /// Имя не может быть длиннее 50 символов
    /// </summary>
    FirstNameTooLongError455 = 455,
    
    /// <summary>
    /// Отчество не может быть длиннее 50 символов
    /// </summary>
    MiddleNameTooLongError456 = 456,
    
    /// <summary>
    /// Пароль не может быть пустым
    /// </summary>
    EmptyPasswordError457 = 457,
    
    /// <summary>
    /// Пароль должен содержать минимум 8 символов
    /// </summary>
    PasswordTooShortError458 = 458,
    
    /// <summary>
    /// Пароль должен содержать заглавную, строчную буквы и цифру
    /// </summary>
    InvalidPasswordFormatError459 = 459,
    
    /// <summary>
    /// Номер телефона не может быть пустым
    /// </summary>
    EmptyPhoneNumberError460 = 460,
    
    /// <summary>
    /// Неверный формат номера телефона
    /// </summary>
    InvalidPhoneFormatError461 = 461,
    
    /// <summary>
    /// Указана недопустимая роль пользователя
    /// </summary>
    InvalidUserRoleError462 = 462,
    
    /// <summary>
    /// Идентификатор пользователя не может быть пустым
    /// </summary>
    EmptyUserIdError463 = 463,
    
    #endregion
    
    #region Ошибки валидации счетчиков (470-479)
    
    /// <summary>
    /// Идентификатор счетчика не может быть пустым
    /// </summary>
    EmptyWaterMeterIdError470 = 470,
    
    /// <summary>
    /// Указано недопустимое расположение счетчика
    /// </summary>
    InvalidWaterMeterPlaceError471 = 471,
    
    /// <summary>
    /// Указан недопустимый тип счетчика
    /// </summary>
    InvalidWaterTypeError472 = 472,
    
    /// <summary>
    /// Заводской номер счетчика не может быть пустым
    /// </summary>
    EmptyFactoryNumberError473 = 473,
    
    /// <summary>
    /// Заводской номер счетчика не может быть длиннее 10 символов
    /// </summary>
    FactoryNumberTooLongError474 = 474,
    
    /// <summary>
    /// Заводской номер должен содержать только буквы и цифры
    /// </summary>
    InvalidFactoryNumberFormatError475 = 475,
    
    /// <summary>
    /// Дата установки счетчика не может быть пустой
    /// </summary>
    EmptyFactoryYearError476 = 476,
    
    /// <summary>
    /// Дата установки счетчика не может быть в будущем
    /// </summary>
    FutureFactoryYearError477 = 477,
    
    #endregion
    
    #region Ошибки валидации показаний счетчиков (480-489)
    
    /// <summary>
    /// Показание счетчика не может быть пустым
    /// </summary>
    EmptyWaterValueError480 = 480,
    
    /// <summary>
    /// Неверный формат показаний
    /// </summary>
    InvalidWaterValueFormatError481 = 481,
    
    /// <summary>
    /// Дата показания не может быть пустой
    /// </summary>
    EmptyReadingDateError482 = 482,
    
    /// <summary>
    /// Дата показания не может быть в будущем
    /// </summary>
    FutureReadingDateError483 = 483,
    
    #endregion

    #endregion

    #region Внутренние ошибки сервера

    /// <summary>
    /// Внутренняя ошибка сервера
    /// </summary>
    InternalServerError501 = 501,

    /// <summary>
    /// Временная недоступность сервиса
    /// </summary>
    ServiceUnavailableError502 = 502

    #endregion
}