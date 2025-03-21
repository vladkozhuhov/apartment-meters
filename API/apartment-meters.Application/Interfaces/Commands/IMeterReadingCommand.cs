using Application.Models.MeterReadingModel;
using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для операций, изменяющих данные показаний водомеров
/// </summary>
public interface IMeterReadingCommand
{
    /// <summary>
    /// Добавляет новое показание водомера
    /// </summary>
    /// <param name="dto">Данные для добавления показания</param>
    /// <returns>Созданная сущность показания водомера</returns>
    /// <exception cref="ArgumentNullException">Если dto равен null</exception>
    /// <exception cref="InvalidOperationException">Если новое показание меньше предыдущего</exception>
    /// <exception cref="FormatException">Если формат показания неверный</exception>
    Task<MeterReadingEntity> AddMeterReadingAsync(MeterReadingAddDto dto);
    
    /// <summary>
    /// Обновляет данные показания водомера
    /// </summary>
    /// <param name="id">Идентификатор показания для обновления</param>
    /// <param name="dto">DTO с обновленными данными показания</param>
    /// <exception cref="ArgumentNullException">Если dto равен null</exception>
    /// <exception cref="KeyNotFoundException">Если показание с указанным id не найдено</exception>
    /// <exception cref="InvalidOperationException">Если новое показание меньше предыдущего</exception>
    /// <exception cref="FormatException">Если формат показания неверный</exception>
    Task UpdateMeterReadingAsync(Guid id, MeterReadingUpdateDto dto);
    
    /// <summary>
    /// Удаляет показание водомера
    /// </summary>
    /// <param name="id">Идентификатор показания для удаления</param>
    /// <exception cref="KeyNotFoundException">Если показание с указанным id не найдено</exception>
    Task DeleteMeterReadingAsync(Guid id);
}