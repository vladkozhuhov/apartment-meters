using System.Net;
using Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// Фильтр исключений API для обработки различных типов исключений
/// </summary>
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;

    /// <summary>
    /// Конструктор с инициализацией обработчиков исключений разных типов
    /// </summary>
    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(BusinessLogicException), HandleBusinessLogicException },
            { typeof(AuthenticationException), HandleAuthenticationException },
            { typeof(MeterReadingValidationException), HandleMeterReadingValidationException },
            { typeof(UserValidationException), HandleUserValidationException },
            { typeof(WaterMeterValidationException), HandleWaterMeterValidationException }
        };
    }

    /// <summary>
    /// Обработка возникшего исключения
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    /// <summary>
    /// Делегирование обработки исключения соответствующему обработчику
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        HandleUnknownException(context);
    }

    /// <summary>
    /// Обработка исключений валидации
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;
        var errors = GetValidationErrors(exception.Errors);
        
        var details = new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка валидации запроса",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "Запрос содержит ошибки валидации",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений "объект не найден"
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Ресурс не найден",
            Status = (int)HttpStatusCode.NotFound,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений запрещённого доступа
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        var exception = (ForbiddenAccessException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Title = "Доступ запрещён",
            Status = (int)HttpStatusCode.Forbidden,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };

        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new ObjectResult(details)
        {
            StatusCode = (int)HttpStatusCode.Forbidden
        };
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений аутентификации
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleAuthenticationException(ExceptionContext context)
    {
        var exception = (AuthenticationException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка аутентификации",
            Status = (int)HttpStatusCode.Unauthorized,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new UnauthorizedObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений валидации показаний счетчиков
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleMeterReadingValidationException(ExceptionContext context)
    {
        var exception = (MeterReadingValidationException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка валидации показаний счетчика",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений валидации пользователей
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleUserValidationException(ExceptionContext context)
    {
        var exception = (UserValidationException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка валидации данных пользователя",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка исключений валидации счетчиков воды
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleWaterMeterValidationException(ExceptionContext context)
    {
        var exception = (WaterMeterValidationException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка валидации счетчика воды",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка бизнес-исключений
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleBusinessLogicException(ExceptionContext context)
    {
        var exception = (BusinessLogicException)context.Exception;
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Ошибка бизнес-логики",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        
        details.Extensions["errorCode"] = (int)exception.ErrorType;
        details.Extensions["errorType"] = exception.ErrorType.ToString();

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка неизвестных исключений
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    private void HandleUnknownException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Необработанное исключение: {ExceptionMessage}", context.Exception.Message);
        
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Внутренняя ошибка сервера",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "Произошла внутренняя ошибка сервера",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Обработка ошибок валидации для формирования структурированного объекта
    /// </summary>
    /// <param name="failures">Список ошибок валидации</param>
    /// <returns>Словарь с ошибками по свойствам</returns>
    private static IDictionary<string, string[]> GetValidationErrors(IEnumerable<ValidationFailure> failures)
    {
        return failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }
}