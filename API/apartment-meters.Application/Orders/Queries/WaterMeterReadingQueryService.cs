using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Реализация сервиса запросов для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingQueryService : IWaterMeterReadingQueryService
{
    private readonly IWaterMeterReadingRepository _waterMeterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="waterMeterReadingRepository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingQueryService(IWaterMeterReadingRepository waterMeterReadingRepository)
    {
        _waterMeterReadingRepository = waterMeterReadingRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetMeterReadingByUserIdAsync(Guid userId)
    {
        return await _waterMeterReadingRepository.GetByUserIdAsync(userId);
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity> GetMeterReadingByIdAsync(Guid id)
    {
        return await _waterMeterReadingRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllMeterReadingAsync()
    {
        return await _waterMeterReadingRepository.GetAllAsync();
    }
}