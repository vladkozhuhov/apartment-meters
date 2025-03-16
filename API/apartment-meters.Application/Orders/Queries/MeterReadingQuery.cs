using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Queries;

/// <summary>
/// Реализация сервиса запросов для работы с показаниями водомеров
/// </summary>
public class MeterReadingQuery : IMeterReadingQuery
{
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly ICachedRepository<MeterReadingEntity, Guid> _cachedRepository;
    private readonly ILogger<MeterReadingQuery> _logger;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="meterReadingRepository">Репозиторий показаний водомеров</param>
    /// <param name="cachedRepository">Кэширующий репозиторий показаний водомеров</param>
    /// <param name="logger">Сервис логирования</param>
    public MeterReadingQuery(
        IMeterReadingRepository meterReadingRepository,
        ICachedRepository<MeterReadingEntity, Guid> cachedRepository,
        ILogger<MeterReadingQuery> logger)
    {
        _meterReadingRepository = meterReadingRepository ?? throw new ArgumentNullException(nameof(meterReadingRepository));
        _cachedRepository = cachedRepository ?? throw new ArgumentNullException(nameof(cachedRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetMeterReadingByWaterMeterIdAsync(Guid waterMeterId)
    {
        _logger.LogInformation("Получение показаний для счетчика с ID {WaterMeterId}", waterMeterId);
        return await _meterReadingRepository.GetByWaterMeterIdAsync(waterMeterId);
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity> GetMeterReadingByIdAsync(Guid id)
    {
        _logger.LogInformation("Получение показания с ID {MeterReadingId}", id);
        return await _cachedRepository.GetByIdCachedAsync(id) ?? 
               await _meterReadingRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllMeterReadingAsync()
    {
        _logger.LogInformation("Получение всех показаний водомеров");
        return await _cachedRepository.GetAllCachedAsync();
    }
}