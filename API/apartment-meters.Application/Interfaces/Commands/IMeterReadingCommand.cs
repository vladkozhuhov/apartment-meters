using Application.Models;
using Application.Models.MeterReadingModel;
using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для операций, изменяющих данные водомеров
/// </summary>
public interface IMeterReadingCommand
{
    /// <summary>
    /// Добавить показание водомера
    /// </summary>
    /// <param name="dto">Данные для добавления показания</param>
    /// <returns>Task</returns>
    Task<MeterReadingEntity> AddMeterReadingAsync(MeterReadingAddDto dto);
    
    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="dto">DTO с обновленными данными пользователя</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateMeterReadingAsync(Guid id, MeterReadingUpdateDto dto);
    
    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Task</returns>
    Task DeleteMeterReadingAsync(Guid id);
}