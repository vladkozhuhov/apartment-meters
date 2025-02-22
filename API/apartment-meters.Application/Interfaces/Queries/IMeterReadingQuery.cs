using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Интерфейс для операций, извлекающих данные водомеров
/// </summary>
public interface IMeterReadingQuery
{
    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Коллекция показаний водомеров</returns>
    Task<IEnumerable<MeterReadingEntity>> GetAllMeterReadingAsync();

    /// <summary>
    /// Получить данные показания водомеров по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные показания водомеров или null, если показания водомеров не найден</returns>
    Task<IEnumerable<MeterReadingEntity>> GetMeterReadingByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить данные показания водомеров по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор показания водомера</param>
    /// <returns>Задача, содержащая данные показания водомеров или null, если показания водомеров не найден</returns>
    Task<MeterReadingEntity> GetMeterReadingByIdAsync(Guid id);
}