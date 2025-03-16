using Domain.Enums;

namespace Application.Exceptions;

public static class ErrorMessages
{
    /// <summary>
    /// Получить текстовое сообщение для типа ошибки
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <returns>Сообщение об ошибке</returns>
    public static string GetMessage(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.UserNotFoundError101 => "Пользователь с таким номером квартиры не найден.",
            ErrorType.InvalidPasswordError102 => "Неправильный пароль.",
            ErrorType.UserBlockedError103 => "Пользователь заблокирован.",
            ErrorType.InvalidTokenError104 => "Токен авторизации недействителен.",
            ErrorType.UserDataNotFoundError201 => "Данные пользователя не найдены.",
            ErrorType.UserUpdateFailedError202 => "Ошибка при обновлении данных пользователя.",
            ErrorType.UserPermissionDeniedError203 => "У пользователя отсутствуют права на это действие.",
            ErrorType.ApartmentDataNotFoundError301 => "Данные квартиры не найдены.",
            ErrorType.UserDeletionFailedError302 => "Невозможно удалить данные по пользователю.",
            ErrorType.WaterMeterNotFoundError351 => "Счетчик с указанным идентификатором не найден.",
            ErrorType.MeterReadingNotFoundError352 => "Показание счетчика с указанным идентификатором не найдено.",
            ErrorType.MeterReadingLessThanPreviousError353 => "Новое показание не может быть меньше предыдущего.",
            ErrorType.InvalidMeterReadingFormatError354 => "Неверный формат показания счетчика. Формат должен быть 'целое,дробное' (до 5 цифр до запятой и до 3 после).",
            ErrorType.InvalidDataFormatError401 => "Неправильный формат данных.",
            ErrorType.MissingRequiredParametersError402 => "Отсутствуют обязательные параметры.",
            ErrorType.InternalServerError501 => "Внутренняя ошибка сервера.",
            ErrorType.ServiceUnavailableError502 => "Временная недоступность сервиса.",
            _ => "Неизвестная ошибка."
        };
    }
}