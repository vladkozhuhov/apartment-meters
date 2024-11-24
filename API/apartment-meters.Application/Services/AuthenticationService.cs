using Application.Interfaces.Commands;
using Application.Models;
using Domain.Repositories;

namespace Application.Services;

/// <summary>
/// Реализация сервиса авторизации
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод для входа в систему
    /// </summary>
    /// <param name="loginDto">Данные для авторизации</param>
    /// <returns>Признак успешной авторизации</returns>
    public async Task<bool> LoginAsync(LoginDto loginDto)
    {
        // Поиск пользователя по номеру квартиры
        var user = await _userRepository.GetByApartmentNumberAsync(loginDto.ApartmentNumber);

        if (user == null)
            return false;

        // Проверка пароля
        return user.Password == loginDto.Password;
    }
}