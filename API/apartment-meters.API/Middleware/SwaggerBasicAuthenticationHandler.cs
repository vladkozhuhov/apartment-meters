using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace API.Middleware;

/// <summary>
/// Обработчик аутентификации для Basic Auth в Swagger
/// </summary>
public class SwaggerBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    
    public SwaggerBasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Аутентификация применяется только для Swagger
        if (!Request.Path.StartsWithSegments("/swagger"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Scheme != "Basic")
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            // Получаем данные из конфигурации
            var swaggerUser = _configuration["Swagger:Username"];
            var swaggerPassword = _configuration["Swagger:Password"];

            // Проверяем учетные данные
            if (username == swaggerUser && password == swaggerPassword)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }
    }
}