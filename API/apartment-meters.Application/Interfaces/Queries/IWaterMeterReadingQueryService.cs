using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Интерфейс для операций, извлекающих данные водомеров
/// </summary>
public interface IWaterMeterReadingQueryService
{
    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Коллекция показаний водомеров</returns>
    Task<IEnumerable<MeterReading>> GetAllMeterReadingAsync();

    /// <summary>
    /// Получить данные показания водомеров по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные показания водомеров или null, если показания водомеров не найден</returns>
    Task<MeterReading> GetMeterReadingByUserIdAsync(Guid userId);
}