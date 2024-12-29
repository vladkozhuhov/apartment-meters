using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с пользователями
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="user">Пользователь для добавления</param>
    /// <returns>Задача, которая завершится после добавления</returns>
    Task<User> AddAsync(User user);
    
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    Task<IEnumerable<User>> GetAllAsync();
    
    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    /// <param name="user">Обновленная информация о пользователе</param>
    /// <returns>Задача, которая завершится после обновления</returns>
    Task UpdateAsync(User user);

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="user">Пользователь для удаления</param>
    /// <returns>Задача, которая завершится после удаления</returns>
    Task DeleteAsync(User user);
    
    /// <summary>
    /// Получить номер квартиры по пользователю
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Номер квартиры</returns>
    Task<User?> GetByApartmentNumberAsync(int apartmentNumber);
}