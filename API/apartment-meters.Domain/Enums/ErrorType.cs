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