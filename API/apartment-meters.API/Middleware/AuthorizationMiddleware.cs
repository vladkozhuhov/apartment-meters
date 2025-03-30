using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API.Middleware;

/// <summary>
/// Middleware для проверки доступа к API независимо от роли пользователя
/// </summary>
public class CustomAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomAuthorizationMiddleware> _logger;

    public CustomAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<CustomAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Пропускаем запросы, которые не требуют авторизации (например, /login)
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        // Проверяем, аутентифицирован ли пользователь
        if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Пользователь не аутентифицирован");
            await _next(context);
            return;
        }

        // Проверяем URL запроса на /api/auth/me
        if (context.Request.Path.StartsWithSegments("/api/auth/me"))
        {
            _logger.LogInformation("Запрос к /api/auth/me обрабатывается middleware");
            
            // Получаем информацию о пользователе
            var userId = context.User.FindFirst("sub")?.Value;
            var username = context.User.FindFirst("name")?.Value;
            var roles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            
            _logger.LogInformation("Роли пользователя из middleware: {Roles}", string.Join(", ", roles));
            
            // Пропускаем запрос дальше без проверки роли
            await _next(context);
            return;
        }

        // Для всех остальных запросов передаем обработку следующему middleware
        await _next(context);
    }
}

/// <summary>
/// Расширение для добавления custom middleware в pipeline
/// </summary>
public static class CustomAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomAuthorizationMiddleware>();
    }
} 