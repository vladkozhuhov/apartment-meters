using Application.Models;

namespace Application.Interfaces.Queries;

/// <summary>
/// Сервис для выполнения запросов, связанных с пользователями
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список пользователей в формате UserDto</returns>
    Task<IEnumerable<UserDto>> GetUsersAsync();

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Данные пользователя в формате UserDto</returns>
    Task<UserDto> GetUserByIdAsync(Guid id);
}