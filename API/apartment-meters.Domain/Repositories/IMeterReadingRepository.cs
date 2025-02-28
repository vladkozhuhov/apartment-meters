using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Интерфейс для работы с показаниями водомеров
/// </summary>
public interface IMeterReadingRepository
{
    /// <summary>
    /// Добавить новое показание водомера
    /// </summary>
    /// <param name="meterReadingEntity">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task<MeterReadingEntity> AddAsync(MeterReadingEntity meterReadingEntity);

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Список показаний водомеров</returns>
    Task<IEnumerable<MeterReadingEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить показание по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Показание водомера</returns>
    Task<IEnumerable<MeterReadingEntity>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить показание по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор показания водомера</param>
    /// <returns>Показание водомера</returns>
    Task<MeterReadingEntity> GetByIdAsync(Guid id);

    /// <summary>
    /// Получить список всех показаний водомеров для пользователя
    /// </summary>
    /// <param name="meterReadingEntity">Обновленная информация о показании водомеров</param>
    /// <returns>Список показаний водомеров</returns>
    Task UpdateAsync(MeterReadingEntity meterReadingEntity);

    /// <summary>
    /// Удалить показание водомера
    /// </summary>
    /// <param name="meterReadingEntity">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task DeleteAsync(MeterReadingEntity meterReadingEntity);
    
    /// <summary>
    /// Получить последнее показание водомера для пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Последнее показание водомера</returns>
    Task<MeterReadingEntity?> GetLastByUserIdAsync(Guid userId);
}