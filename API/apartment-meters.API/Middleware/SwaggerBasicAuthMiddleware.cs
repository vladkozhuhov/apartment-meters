using System.Net;
using System.Text;

namespace API.Middleware;

/// <summary>
/// Middleware для базовой аутентификации Swagger UI
/// </summary>
public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Проверяем, является ли запрос к Swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string authHeader = context.Request.Headers["Authorization"];
            
            // Если заголовок авторизации отсутствует, запрашиваем аутентификацию
            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Swagger\"");
                return;
            }

            // Обрабатываем Basic Authentication
            if (authHeader.StartsWith("Basic "))
            {
                string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string credentials = encoding.GetString(Convert.FromBase64String(encodedCredentials));
                
                // Получаем логин и пароль из строки "username:password"
                int separatorIndex = credentials.IndexOf(':');
                if (separatorIndex > 0)
                {
                    string username = credentials.Substring(0, separatorIndex);
                    string password = credentials.Substring(separatorIndex + 1);
                    
                    // Получаем настройки аутентификации из конфигурации
                    string configUsername = _configuration["Swagger:Username"];
                    string configPassword = _configuration["Swagger:Password"];
                    
                    // Проверяем учетные данные
                    if (username == configUsername && password == configPassword)
                    {
                        await _next(context);
                        return;
                    }
                }
            }
            
            // Если аутентификация не прошла, возвращаем 401
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Swagger\"");
            return;
        }
        
        // Если запрос не к Swagger, пропускаем его дальше
        await _next(context);
    }
}