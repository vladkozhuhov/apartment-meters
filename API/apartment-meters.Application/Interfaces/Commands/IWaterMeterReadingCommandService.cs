using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для операций, изменяющих данные водомеров
/// </summary>
public interface IWaterMeterReadingCommandService
{
    /// <summary>
    /// Добавить показание водомера
    /// </summary>
    /// <param name="meterReading">Сущность показания водомера</param>
    /// <returns>Task</returns>
    Task AddMeterReadingAsync(MeterReading meterReading);
}