using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Models.LoginModels;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace Application.Services;

/// <summary>
/// Реализация сервиса авторизации
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ICachedRepository<UserEntity, Guid> _cachedRepository;
    private readonly ILogger<AuthenticationService> _logger;

    /// <summary>
    /// Конструктор сервиса аутентификации
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей</param>
    /// <param name="cachedRepository">Кэширующий репозиторий пользователей</param>
    /// <param name="logger">Сервис логирования</param>
    public AuthenticationService(
        IUserRepository userRepository,
        ICachedRepository<UserEntity, Guid> cachedRepository,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _cachedRepository = cachedRepository ?? throw new ArgumentNullException(nameof(cachedRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Метод для входа в систему
    /// </summary>
    /// <param name="loginDto">Данные для авторизации</param>
    /// <returns>Информация о пользователе при успешной авторизации</returns>
    /// <exception cref="BusinessLogicException">Выбрасывается при неверных учетных данных</exception>
    public async Task<UserEntity> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Попытка входа пользователя с номером квартиры {ApartmentNumber}", loginDto.ApartmentNumber);

        var user = await _userRepository.GetByApartmentNumberAsync(loginDto.ApartmentNumber);
        if (user == null)
        {
            _logger.LogWarning("Пользователь с номером квартиры {ApartmentNumber} не найден", loginDto.ApartmentNumber);
            throw new BusinessLogicException(ErrorType.UserNotFoundError101);
        }

        // Проверяем пароль с использованием BCrypt
        if (!VerifyPassword(loginDto.Password, user.Password))
        {
            _logger.LogWarning("Неверный пароль для пользователя с номером квартиры {ApartmentNumber}", loginDto.ApartmentNumber);
            throw new BusinessLogicException(ErrorType.InvalidPasswordError102);
        }

        // Кэшируем пользователя после успешной аутентификации
        await _cachedRepository.GetByIdCachedAsync(user.Id);

        _logger.LogInformation("Успешный вход пользователя с номером квартиры {ApartmentNumber}", loginDto.ApartmentNumber);
        return user;
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