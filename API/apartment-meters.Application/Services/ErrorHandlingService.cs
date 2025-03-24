using Application.Exceptions;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Сервис для обработки и преобразования ошибок валидации 
/// </summary>
public interface IErrorHandlingService
{
    /// <summary>
    /// Преобразует ошибки валидации в бизнес-исключение с соответствующим кодом ошибки
    /// </summary>
    /// <param name="failures">Список ошибок валидации</param>
    /// <exception cref="BusinessLogicException">Исключение с кодом ошибки</exception>
    void ThrowValidationException(IEnumerable<ValidationFailure> failures);
    
    /// <summary>
    /// Бросает исключение с указанным типом ошибки
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <exception cref="BusinessLogicException">Исключение с кодом ошибки</exception>
    void ThrowBusinessLogicException(ErrorType errorType);
    
    /// <summary>
    /// Бросает исключение с указанным типом ошибки и сообщением
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <exception cref="BusinessLogicException">Исключение с кодом ошибки</exception>
    void ThrowBusinessLogicException(ErrorType errorType, string message);
    
    /// <summary>
    /// Бросает исключение "не найдено" с указанным типом ошибки
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <exception cref="NotFoundException">Исключение с кодом ошибки</exception>
    void ThrowNotFoundException(ErrorType errorType);
    
    /// <summary>
    /// Бросает исключение "не найдено" с указанным типом ошибки и сообщением
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <exception cref="NotFoundException">Исключение с кодом ошибки</exception>
    void ThrowNotFoundException(ErrorType errorType, string message);
    
    /// <summary>
    /// Бросает исключение "запрещенный доступ" с указанным типом ошибки
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <exception cref="ForbiddenAccessException">Исключение с кодом ошибки</exception>
    void ThrowForbiddenAccessException(ErrorType errorType);
    
    /// <summary>
    /// Бросает исключение "запрещенный доступ" с указанным типом ошибки и сообщением
    /// </summary>
    /// <param name="errorType">Тип ошибки</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <exception cref="ForbiddenAccessException">Исключение с кодом ошибки</exception>
    void ThrowForbiddenAccessException(ErrorType errorType, string message);
}

/// <summary>
/// Реализация сервиса обработки ошибок
/// </summary>
public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;

    /// <summary>
    /// Конструктор сервиса обработки ошибок
    /// </summary>
    /// <param name="logger">Сервис логирования</param>
    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void ThrowValidationException(IEnumerable<ValidationFailure> failures)
    {
        // Логируем ошибки валидации
        foreach (var failure in failures)
        {
            _logger.LogWarning("Ошибка валидации: {Property} {Error}", failure.PropertyName, failure.ErrorMessage);
        }
        
        // Преобразуем ошибки в исключение ValidationException
        throw new ValidationException(failures);
    }

    /// <inheritdoc/>
    public void ThrowBusinessLogicException(ErrorType errorType)
    {
        _logger.LogWarning("Бизнес-исключение: {ErrorType} {Message}", errorType, errorType.GetMessage());
        throw new BusinessLogicException(errorType);
    }

    /// <inheritdoc/>
    public void ThrowBusinessLogicException(ErrorType errorType, string message)
    {
        _logger.LogWarning("Бизнес-исключение: {ErrorType} {Message}", errorType, message);
        throw new BusinessLogicException(errorType, message);
    }

    /// <inheritdoc/>
    public void ThrowNotFoundException(ErrorType errorType)
    {
        _logger.LogWarning("Исключение 'не найдено': {ErrorType} {Message}", errorType, errorType.GetMessage());
        throw new NotFoundException(errorType);
    }

    /// <inheritdoc/>
    public void ThrowNotFoundException(ErrorType errorType, string message)
    {
        _logger.LogWarning("Исключение 'не найдено': {ErrorType} {Message}", errorType, message);
        throw new NotFoundException(message, errorType);
    }

    /// <inheritdoc/>
    public void ThrowForbiddenAccessException(ErrorType errorType)
    {
        _logger.LogWarning("Исключение 'запрещенный доступ': {ErrorType} {Message}", errorType, errorType.GetMessage());
        throw new ForbiddenAccessException(errorType.GetMessage(), errorType);
    }

    /// <inheritdoc/>
    public void ThrowForbiddenAccessException(ErrorType errorType, string message)
    {
        _logger.LogWarning("Исключение 'запрещенный доступ': {ErrorType} {Message}", errorType, message);
        throw new ForbiddenAccessException(message, errorType);
    }
} 