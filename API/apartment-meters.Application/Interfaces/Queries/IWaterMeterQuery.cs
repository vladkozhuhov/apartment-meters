using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Сервис для выполнения запросов, связанных с счетчиками
/// </summary>
public interface IWaterMeterQuery
{
    /// <summary>
    /// Получить данные счетчиков по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные счетчика или null, если счетчики не найден</returns>
    Task<IEnumerable<WaterMeterEntity>> GetWaterMeterByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить данные счетчика
    /// </summary>
    /// <param name="id">Идентификатор счетчика</param>
    /// <returns>Задача, содержащая данные счетчика или null, если счетчик не найден</returns>
    Task<WaterMeterEntity> GetWaterMeterByIdAsync(Guid id);
}