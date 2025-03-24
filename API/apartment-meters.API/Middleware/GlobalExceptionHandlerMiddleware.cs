using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace API.Middleware;

/// <summary>
/// Middleware для глобальной обработки исключений
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Необработанное исключение: {Message}", exception.Message);

        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = 500;
        var errorMessage = "Внутренняя ошибка сервера";
        
        // Определяем тип исключения для предоставления соответствующего HTTP статус-кода
        switch (exception)
        {
            case BusinessLogicException businessEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = (int)businessEx.ErrorType;
                errorMessage = businessEx.Message;
                break;
                
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorCode = 404;
                errorMessage = exception.Message;
                break;
                
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = 400;
                errorMessage = exception.Message;
                break;
                
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = 400;
                errorMessage = exception.Message;
                break;
                
            default:
                // В продакшн-режиме не показываем детали внутренних ошибок
                if (_env.IsDevelopment())
                {
                    errorMessage = $"{exception.Message}\n{exception.StackTrace}";
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new
        {
            Code = errorCode,
            Message = errorMessage
        });

        await context.Response.WriteAsync(response);
    }
} 