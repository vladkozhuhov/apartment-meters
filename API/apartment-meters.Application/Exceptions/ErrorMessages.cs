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
            
            // Ошибки валидации пользователей
            ErrorType.EmptyApartmentNumberError450 => "Номер квартиры не может быть пустым.",
            ErrorType.InvalidApartmentNumberError451 => "Номер квартиры должен быть положительным числом.",
            ErrorType.EmptyLastNameError452 => "Фамилия не может быть пустой.",
            ErrorType.LastNameTooLongError453 => "Фамилия не может быть длиннее 50 символов.",
            ErrorType.EmptyFirstNameError454 => "Имя не может быть пустым.",
            ErrorType.FirstNameTooLongError455 => "Имя не может быть длиннее 50 символов.",
            ErrorType.MiddleNameTooLongError456 => "Отчество не может быть длиннее 50 символов.",
            ErrorType.EmptyPasswordError457 => "Пароль не может быть пустым.",
            ErrorType.PasswordTooShortError458 => "Пароль должен содержать минимум 8 символов.",
            ErrorType.InvalidPasswordFormatError459 => "Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру.",
            ErrorType.EmptyPhoneNumberError460 => "Номер телефона не может быть пустым.",
            ErrorType.InvalidPhoneFormatError461 => "Номер телефона должен быть в формате +7XXXXXXXXXX.",
            ErrorType.InvalidUserRoleError462 => "Указана недопустимая роль пользователя.",
            ErrorType.EmptyUserIdError463 => "Идентификатор пользователя не может быть пустым.",
            
            // Ошибки валидации счетчиков
            ErrorType.EmptyWaterMeterIdError470 => "Идентификатор счетчика не может быть пустым.",
            ErrorType.InvalidWaterMeterPlaceError471 => "Указано недопустимое расположение счетчика.",
            ErrorType.InvalidWaterTypeError472 => "Указан недопустимый тип счетчика.",
            ErrorType.EmptyFactoryNumberError473 => "Заводской номер счетчика не может быть пустым.",
            ErrorType.FactoryNumberTooLongError474 => "Заводской номер счетчика не может быть длиннее 10 символов.",
            ErrorType.InvalidFactoryNumberFormatError475 => "Заводской номер должен содержать только буквы и цифры.",
            ErrorType.EmptyFactoryYearError476 => "Дата установки счетчика не может быть пустой.",
            ErrorType.FutureFactoryYearError477 => "Дата установки счетчика не может быть в будущем.",
            
            // Ошибки валидации показаний счетчиков
            ErrorType.EmptyWaterValueError480 => "Показание счетчика не может быть пустым.",
            ErrorType.InvalidWaterValueFormatError481 => "Формат показаний должен содержать до 5 цифр до запятой и до 3 после.",
            ErrorType.EmptyReadingDateError482 => "Дата показания не может быть пустой.",
            ErrorType.FutureReadingDateError483 => "Дата показания не может быть в будущем.",
            
            ErrorType.InternalServerError501 => "Внутренняя ошибка сервера.",
            ErrorType.ServiceUnavailableError502 => "Временная недоступность сервиса.",
            _ => "Неизвестная ошибка."
        };
    }
}