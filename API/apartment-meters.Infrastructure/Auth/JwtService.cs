using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IOptions<JwtSettings> jwtSettings, ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public string GenerateToken(string userId, string username, IEnumerable<string> roles)
    {
        _logger.LogInformation("Генерация JWT токена для пользователя {UserId} с ролями {Roles}", userId, string.Join(", ", roles));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // Добавляем номер квартиры как отдельный claim
            new Claim("apartmentNumber", username),
            
            // Добавляем claim для совместимости с различными системами
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username)
        };

        // Добавляем роли отдельными claim'ами
        foreach (var role in roles)
        {
            _logger.LogInformation("Добавление роли {Role} в JWT токен", role);
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("JWT токен успешно сгенерирован");
        return tokenString;
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        _logger.LogInformation("Получение ClaimsPrincipal из токена");
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            
            // Проверяем роли в principal
            var roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            _logger.LogInformation("Токен успешно валидирован. Роли: {Roles}", string.Join(", ", roles));
            
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка валидации токена: {ErrorMessage}", ex.Message);
            return null;
        }
    }

    public string? ValidateToken(string token)
    {
        _logger.LogInformation("Валидация токена");
        
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Токен пустой или null");
            return null;
        }

        var principal = GetPrincipalFromToken(token);
        if (principal == null)
        {
            _logger.LogWarning("Не удалось получить ClaimsPrincipal из токена");
            return null;
        }

        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        _logger.LogInformation("Токен валидирован успешно для пользователя {UserId}", userId);
        return userId;
    }
}