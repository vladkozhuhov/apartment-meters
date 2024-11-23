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
    /// <returns>Список показаний водомеров</returns>
    Task<IEnumerable<MeterReading>> GetAllMeterReadingsAsync();

    /// <summary>
    /// Получить показание водомера по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Сущность показания водомера</returns>
    Task<MeterReading?> GetMeterReadingByIdAsync(Guid id);
}