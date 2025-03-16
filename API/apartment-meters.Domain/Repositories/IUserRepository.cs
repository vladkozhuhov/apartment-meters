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
    /// <param name="userEntity">Пользователь для добавления</param>
    /// <returns>Задача, которая завершится после добавления</returns>
    Task<UserEntity> AddAsync(UserEntity userEntity);
    
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    Task<IEnumerable<UserEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<UserEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    /// <param name="userEntity">Обновленная информация о пользователе</param>
    /// <returns>Задача, которая завершится после обновления</returns>
    Task UpdateAsync(UserEntity userEntity);

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="userEntity">Пользователь для удаления</param>
    /// <returns>Задача, которая завершится после удаления</returns>
    Task DeleteAsync(UserEntity userEntity);
    
    /// <summary>
    /// Получить пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<UserEntity?> GetByApartmentNumberAsync(int apartmentNumber);
}