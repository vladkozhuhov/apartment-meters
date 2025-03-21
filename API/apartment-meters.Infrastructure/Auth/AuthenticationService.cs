using Application.Interfaces.Services;
using Application.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence.Contexts;

namespace Infrastructure.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(
        ApplicationDbContext dbContext,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> AuthenticateAsync(string apartmentNumber, string password)
    {
        // Проверяем, что номер квартиры может быть преобразован в int
        if (!int.TryParse(apartmentNumber, out int apartmentNumberInt))
        {
            return null; // Возвращаем null, если номер квартиры не является числом
        }

        // Поиск пользователя по номеру квартиры
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.ApartmentNumber == apartmentNumberInt);

        if (user == null)
        {
            return null;
        }

        // Проверка пароля (в реальном проекте должна быть проверка хеша)
        if (user.Password != password)
        {
            return null;
        }

        // Определяем роли на основе Role пользователя
        var roles = new List<string> { user.Role.ToString() };

        // Генерация токена
        var token = _jwtService.GenerateToken(
            user.Id.ToString(),
            user.ApartmentNumber.ToString(),
            roles
        );

        // Создание ответа
        return new LoginResponse
        {
            Token = token,
            Username = user.ApartmentNumber.ToString(),
            Roles = roles,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
        };
    }
}