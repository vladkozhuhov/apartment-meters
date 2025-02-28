using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Сервис для выполнения операций чтения счетчика
/// </summary>
public class WaterMeterQuery : IWaterMeterQuery
{
    private readonly IWaterMeterRepository _waterMaterRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="waterMaterRepository">Репозиторий для работы с сущностью WaterMeter</param>
    public WaterMeterQuery(IWaterMeterRepository waterMaterRepository)
    {
        _waterMaterRepository = waterMaterRepository;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<WaterMeterEntity>> GetWaterMeterByUserIdAsync(Guid userId)
    {
        return await _waterMaterRepository.GetByUserIdAsync(userId);
    }
    
    /// <inheritdoc />
    public async Task<WaterMeterEntity> GetWaterMeterByIdAsync(Guid id)
    {
        return await _waterMaterRepository.GetByIdAsync(id);
    }
}