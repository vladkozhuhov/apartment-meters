using API.Middleware;

namespace API.Extensions;

/// <summary>
/// Расширения для настройки middleware в приложении
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Добавляет глобальный обработчик исключений в конвейер запросов
    /// </summary>
    /// <param name="app">Web-приложение</param>
    /// <returns>Web-приложение с настроенным обработчиком исключений</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
} 