using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Интерфейс для работы с показаниями водомеров
/// </summary>
public interface IWaterMeterReadingRepository
{
    /// <summary>
    /// Добавить новое показание водомера
    /// </summary>
    /// <param name="meterReading">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task<MeterReading> AddAsync(MeterReading meterReading);

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Список показаний водомеров</returns>
    Task<IEnumerable<MeterReading>> GetAllAsync();
    
    /// <summary>
    /// Получить показание по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Показание водомера</returns>
    Task<IEnumerable<MeterReading>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить показание по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор показания водомера</param>
    /// <returns>Показание водомера</returns>
    Task<MeterReading> GetByIdAsync(Guid id);

    /// <summary>
    /// Получить список всех показаний водомеров для пользователя
    /// </summary>
    /// <param name="meterReading">Обновленная информация о показании водомеров</param>
    /// <returns>Список показаний водомеров</returns>
    Task UpdateAsync(MeterReading meterReading);

    /// <summary>
    /// Удалить показание водомера
    /// </summary>
    /// <param name="meterReading">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task DeleteAsync(MeterReading meterReading);
}