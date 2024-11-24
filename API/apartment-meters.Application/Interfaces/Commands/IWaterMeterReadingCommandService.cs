using Application.Models;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для операций, изменяющих данные водомеров
/// </summary>
public interface IWaterMeterReadingCommandService
{
    /// <summary>
    /// Добавить показание водомера
    /// </summary>
    /// <param name="dto">Данные для добавления показания</param>
    /// <returns>Task</returns>
    Task AddMeterReadingAsync(AddWaterMeterReadingDto dto);
    
    /// <summary>
    /// Удалить показание.
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Task</returns>
    Task DeleteMeterReadingAsync(Guid id);
}