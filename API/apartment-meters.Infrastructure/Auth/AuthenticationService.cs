using Application.Interfaces.Services;
using Application.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence.Contexts;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using BCrypt.Net;

namespace Infrastructure.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        ApplicationDbContext dbContext,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthenticationService> logger)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> AuthenticateAsync(string apartmentNumber, string password)
    {
        _logger.LogInformation("Попытка аутентификации. Номер квартиры: {ApartmentNumber}", apartmentNumber);
        
        // Проверяем, что номер квартиры может быть преобразован в int
        if (!int.TryParse(apartmentNumber, out int apartmentNumberInt))
        {
            _logger.LogWarning("Неверный формат номера квартиры: {ApartmentNumber}", apartmentNumber);
            return null; // Возвращаем null, если номер квартиры не является числом
        }

        // Поиск пользователя по номеру квартиры
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.ApartmentNumber == apartmentNumberInt);

        if (user == null)
        {
            _logger.LogWarning("Пользователь с номером квартиры {ApartmentNumber} не найден", apartmentNumber);
            return null;
        }

        _logger.LogInformation("Пользователь найден. ID: {UserId}, Роль: {Role}", user.Id, user.Role);

        // Проверка пароля с использованием BCrypt или прямое сравнение для обратной совместимости
        if (!VerifyPassword(password, user.Password))
        {
            _logger.LogWarning("Неверный пароль для пользователя с номером квартиры {ApartmentNumber}", apartmentNumber);
            return null;
        }

        // Определяем роли на основе Role пользователя
        var roles = new List<string> { user.Role.ToString() };
        _logger.LogInformation("Роли пользователя: {Roles}", string.Join(", ", roles));

        // Генерация токена
        var token = _jwtService.GenerateToken(
            user.Id.ToString(),
            user.ApartmentNumber.ToString(),
            roles
        );
        
        _logger.LogInformation("Токен сгенерирован для пользователя {UserId}. Роли: {Roles}", user.Id, string.Join(", ", roles));

        // Создание ответа
        var response = new LoginResponse
        {
            Token = token,
            Username = user.ApartmentNumber.ToString(),
            Roles = roles,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
        };
        
        _logger.LogInformation("Аутентификация успешна для пользователя {UserId}", user.Id);
        return response;
    }
    
    /// <summary>
    /// Проверяет соответствие пароля хешу
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <param name="passwordHash">Хеш пароля</param>
    /// <returns>true если пароль верный, иначе false</returns>
    private bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            // Проверяем, начинается ли пароль с префикса BCrypt
            if (passwordHash.StartsWith("$2a$") || passwordHash.StartsWith("$2b$") || passwordHash.StartsWith("$2y$"))
            {
                // Верифицируем хешированный пароль
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            else
            {
                // Для обратной совместимости - простое сравнение, если пароль еще не хеширован
                // Это временное решение для существующих пользователей
                return password == passwordHash;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке пароля");
            return false;
        }
    }
}