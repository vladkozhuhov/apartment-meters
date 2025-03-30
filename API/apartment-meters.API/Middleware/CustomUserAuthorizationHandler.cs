using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Auth;

namespace API.Middleware;

/// <summary>
/// Обработчик авторизации для API/auth/me
/// </summary>
public class ApiAuthMeAuthorizationHandler : AuthorizationHandler<ApiAuthMeRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiAuthMeAuthorizationHandler> _logger;

    public ApiAuthMeAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiAuthMeAuthorizationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApiAuthMeRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        // Проверяем, что запрос идет к API/auth/me
        var path = httpContext.Request.Path.Value?.ToLowerInvariant();
        if (path == null || !path.StartsWith("/api/auth/me"))
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation("Обработка доступа к /api/auth/me через AuthorizationHandler");

        // Если пользователь аутентифицирован, разрешаем ему доступ независимо от роли
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            var roles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            
            _logger.LogInformation("Роли пользователя: {Roles}, предоставляем доступ", string.Join(", ", roles));
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
} 