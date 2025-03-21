using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Сервис для выполнения запросов, связанных с пользователями
/// </summary>
public interface IUserQuery
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Коллекция пользователей</returns>
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    
    /// <summary>
    /// Получить данные пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<UserEntity> GetUserByIdAsync(Guid id);
    
    /// <summary>
    /// Получить данные пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<UserEntity> GetUserByApartmentNumberAsync(int apartmentNumber);
}