using Application.Models.Auth;

namespace Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Аутентификация пользователя по номеру квартиры и паролю
        /// </summary>
        /// <param name="apartmentNumber">Номер квартиры (логин)</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ответ с токеном доступа или null, если аутентификация не удалась</returns>
        Task<LoginResponse> AuthenticateAsync(string apartmentNumber, string password);
    }
} 