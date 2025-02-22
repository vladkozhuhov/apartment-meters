using Application.Models.WaterMeterModel;
using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для управления командами, связанными со счетчиками
/// </summary>
public interface IWaterMeterCommand
{
    /// <summary>
    /// Добавить новый счетчик
    /// </summary>
    /// <param name="dto">DTO с данными нового счетчика</param>
    /// <returns>Идентификатор добавленного счетчика</returns>
    Task<WaterMeterEntity> AddWaterMeterAsync(WaterMeterAddDto dto);
    
    /// <summary>
    /// Обновить данные счетчика
    /// </summary>
    /// <param name="waterMeterId">Идентификатор счетчика</param>
    /// <param name="dto">DTO с обновленными данными счетчика</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateWaterMeterAsync(Guid waterMeterId, WaterMeterUpdateDto dto);

    /// <summary>
    /// Удалить счетчика
    /// </summary>
    /// <param name="id">Идентификатор счетчиками для удаления</param>
    /// <returns>Task для отслеживания операции</returns>
    Task DeleteWaterMeterAsync(Guid id);
}