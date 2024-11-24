using Application.Models;

namespace Application.Interfaces.Commands;

/// <summary>
/// Сервис для авторизации пользователей
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Метод для входа в систему
    /// </summary>
    /// <param name="loginDto">Данные для авторизации</param>
    /// <returns>Признак успешной авторизации</returns>
    Task<bool> LoginAsync(LoginDto loginDto);
}