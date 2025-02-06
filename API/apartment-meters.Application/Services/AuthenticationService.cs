using Application.Interfaces.Commands;
using Application.Models;
using Application.Models.Login;
using Domain.Entities;
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
    public async Task<UserEntity> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByApartmentNumberAsync(loginDto.ApartmentNumber);

        if (user == null || user.Password != loginDto.Password)
            return null;

        return user;
    }
}