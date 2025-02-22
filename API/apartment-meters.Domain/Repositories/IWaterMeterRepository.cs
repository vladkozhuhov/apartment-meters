using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Интерфейс для работы со счетчиками
/// </summary>
public interface IWaterMeterRepository
{
    /// <summary>
    /// Добавить новое счетчика
    /// </summary>
    /// <param name="waterMeterEntity">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task<WaterMeterEntity> AddAsync(WaterMeterEntity waterMeterEntity);
    
    /// <summary>
    /// Получить счетчики по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Счетчик</returns>
    Task<IEnumerable<WaterMeterEntity>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить счетчик по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор счетчика</param>
    /// <returns>Показание счетчика</returns>
    Task<WaterMeterEntity> GetByIdAsync(Guid id);

    /// <summary>
    /// Изменение информации о счетчике
    /// </summary>
    /// <param name="waterMeterEntity">Обновленная информация о счетчике</param>
    /// <returns>Список счетчиков</returns>
    Task UpdateAsync(WaterMeterEntity waterMeterEntity);

    /// <summary>
    /// Удалить счетчик
    /// </summary>
    /// <param name="waterMeterEntity">Сущность счетчика</param>
    /// <returns>Task</returns>
    Task DeleteAsync(WaterMeterEntity waterMeterEntity);
}