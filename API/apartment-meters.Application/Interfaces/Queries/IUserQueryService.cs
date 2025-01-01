using Application.Models;
using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Сервис для выполнения запросов, связанных с пользователями
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Коллекция пользователей</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();
    
    /// <summary>
    /// Получить данные пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<User> GetUserByIdAsync(Guid id);
}