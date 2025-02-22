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
    /// <returns>Задача, содержащая данные счетчика или null, если пользователь не найден</returns>
    Task<WaterMeterEntity> GetUserByIdAsync(Guid userId);
}