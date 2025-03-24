using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

/// <summary>
/// Поведение для автоматической валидации запросов
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IErrorHandlingService _errorHandlingService;

    /// <summary>
    /// Инициализирует поведение валидации
    /// </summary>
    /// <param name="validators">Коллекция валидаторов для запроса</param>
    /// <param name="errorHandlingService">Сервис обработки ошибок</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IErrorHandlingService errorHandlingService)
    {
        _validators = validators;
        _errorHandlingService = errorHandlingService;
    }

    /// <summary>
    /// Обрабатывает запрос с валидацией
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="next">Следующий обработчик</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат обработки запроса</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Если нет валидаторов, просто продолжаем обработку
        if (!_validators.Any()) 
            return await next();
            
        // Получаем контекст валидации
        var context = new ValidationContext<TRequest>(request);
        
        // Выполняем все валидаторы параллельно
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
        // Собираем все ошибки валидации
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
            
        // Если есть ошибки валидации, бросаем исключение
        if (failures.Any())
            _errorHandlingService.ThrowValidationException(failures);
            
        // Если валидация прошла успешно, продолжаем обработку
        return await next();
    }
} 