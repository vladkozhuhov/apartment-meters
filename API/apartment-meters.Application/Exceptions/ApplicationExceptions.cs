using Domain.Enums;

namespace Application.Exceptions;

/// <summary>
/// Базовое исключение для бизнес-логики
/// </summary>
public class BusinessLogicException : Exception
{
    /// <summary>
    /// Тип ошибки
    /// </summary>
    public ErrorType ErrorType { get; }

    /// <summary>
    /// Инициализирует исключение с сообщением и типом ошибки
    /// </summary>
    public BusinessLogicException(ErrorType errorType, string message) : base(message)
    {
        ErrorType = errorType;
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки и стандартным сообщением
    /// </summary>
    public BusinessLogicException(ErrorType errorType) : base(errorType.GetMessage())
    {
        ErrorType = errorType;
    }
}

/// <summary>
/// Исключение для случаев, когда запрашиваемый объект не найден
/// </summary>
public class NotFoundException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с именем сущности и идентификатором
    /// </summary>
    public NotFoundException(string name, object key)
        : base(ErrorType.InvalidDataFormatError401, $"Сущность \"{name}\" ({key}) не найдена.")
    {
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки
    /// </summary>
    public NotFoundException(ErrorType errorType)
        : base(errorType)
    {
    }

    /// <summary>
    /// Инициализирует исключение с сообщением и типом ошибки
    /// </summary>
    public NotFoundException(string message, ErrorType errorType)
        : base(errorType, message)
    {
    }
}

/// <summary>
/// Исключение для случаев, когда доступ запрещен
/// </summary>
public class ForbiddenAccessException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с сообщением
    /// </summary>
    public ForbiddenAccessException(string message = "У вас нет прав для выполнения этого действия.", 
        ErrorType errorType = ErrorType.UserPermissionDeniedError203) 
        : base(errorType, message)
    {
    }
}

/// <summary>
/// Исключение для ошибок аутентификации
/// </summary>
public class AuthenticationException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с типом ошибки
    /// </summary>
    public AuthenticationException(ErrorType errorType) : base(errorType)
    {
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки и сообщением
    /// </summary>
    public AuthenticationException(ErrorType errorType, string message) : base(errorType, message)
    {
    }
}

/// <summary>
/// Исключение для ошибок валидации показаний счетчиков
/// </summary>
public class MeterReadingValidationException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с типом ошибки
    /// </summary>
    public MeterReadingValidationException(ErrorType errorType) : base(errorType)
    {
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки и сообщением
    /// </summary>
    public MeterReadingValidationException(ErrorType errorType, string message) : base(errorType, message)
    {
    }
}

/// <summary>
/// Исключение для ошибок валидации данных пользователя
/// </summary>
public class UserValidationException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с типом ошибки
    /// </summary>
    public UserValidationException(ErrorType errorType) : base(errorType)
    {
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки и сообщением
    /// </summary>
    public UserValidationException(ErrorType errorType, string message) : base(errorType, message)
    {
    }
}

/// <summary>
/// Исключение для ошибок валидации счетчиков воды
/// </summary>
public class WaterMeterValidationException : BusinessLogicException
{
    /// <summary>
    /// Инициализирует исключение с типом ошибки
    /// </summary>
    public WaterMeterValidationException(ErrorType errorType) : base(errorType)
    {
    }

    /// <summary>
    /// Инициализирует исключение с типом ошибки и сообщением
    /// </summary>
    public WaterMeterValidationException(ErrorType errorType, string message) : base(errorType, message)
    {
    }
} 