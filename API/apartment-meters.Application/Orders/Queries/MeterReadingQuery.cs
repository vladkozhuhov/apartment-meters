using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Реализация сервиса запросов для работы с показаниями водомеров
/// </summary>
public class MeterReadingQuery : IMeterReadingQuery
{
    private readonly IMeterReadingRepository _meterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="meterReadingRepository">Репозиторий показаний водомеров</param>
    public MeterReadingQuery(IMeterReadingRepository meterReadingRepository)
    {
        _meterReadingRepository = meterReadingRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetMeterReadingByWaterMeterIdAsync(Guid waterMeterId)
    {
        return await _meterReadingRepository.GetByWaterMeterIdAsync(waterMeterId);
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity> GetMeterReadingByIdAsync(Guid id)
    {
        return await _meterReadingRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllMeterReadingAsync()
    {
        return await _meterReadingRepository.GetAllAsync();
    }
}