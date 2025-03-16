using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Models.LoginModels;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Validators;

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
    /// <exception cref="CustomException">Выбрасывается при неверных учетных данных</exception>
    public async Task<UserEntity> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Попытка входа в систему для квартиры {ApartmentNumber}", loginDto.ApartmentNumber);
        
        var user = await _userRepository.GetByApartmentNumberAsync(loginDto.ApartmentNumber);

        if (user == null)
        {
            _logger.LogWarning("Пользователь с номером квартиры {ApartmentNumber} не найден", loginDto.ApartmentNumber);
            throw new CustomException(ErrorType.UserNotFoundError101);
        }

        if (user.Password != loginDto.Password)
        {
            _logger.LogWarning("Неверный пароль для пользователя с номером квартиры {ApartmentNumber}", loginDto.ApartmentNumber);
            throw new CustomException(ErrorType.InvalidPasswordError102);
        }

        // Кэшируем пользователя после успешной аутентификации
        await _cachedRepository.GetByIdCachedAsync(user.Id);
        
        _logger.LogInformation("Успешный вход в систему для пользователя {UserName} из квартиры {ApartmentNumber}", 
            $"{user.LastName} {user.FirstName}", loginDto.ApartmentNumber);
        return user;
    }
}