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
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Коллекция показаний водомеров</returns>
    Task<IEnumerable<MeterReading>> GetAllMeterReadingAsync();
    
    /// <summary>
    /// Получить данные показания водомеров по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные показания водомеров или null, если показания водомеров не найден</returns>
    Task<MeterReading> GetMeterReadingByIdAsync(Guid id);
    
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