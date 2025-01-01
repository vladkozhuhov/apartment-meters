using Application.Models;
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
    /// <param name="dto">Данные для добавления показания</param>
    /// <returns>Task</returns>
    Task<MeterReading> AddMeterReadingAsync(AddWaterMeterReadingDto dto);
    
    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="dto">DTO с обновленными данными пользователя</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateMeterReadingAsync(UpdateWaterMeterReadingDto dto);
    
    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Task</returns>
    Task DeleteMeterReadingAsync(Guid id);
}