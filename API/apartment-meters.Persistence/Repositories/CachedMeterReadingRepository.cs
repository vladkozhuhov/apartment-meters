using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Repositories;

namespace Persistence.Repositories;

/// <summary>
/// Репозиторий для показаний счетчиков с поддержкой кэширования
/// </summary>
public class CachedMeterReadingRepository : ICachedRepository<MeterReadingEntity, Guid>
{
    private readonly IMeterReadingRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedMeterReadingRepository> _logger;
    
    private const string AllMeterReadingsCacheKey = "all_meter_readings";
    private static readonly string MeterReadingByIdCacheKeyPrefix = "meter_reading_";
    
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="repository">Базовый репозиторий показаний счетчиков</param>
    /// <param name="cache">Сервис кэширования</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedMeterReadingRepository(
        IMeterReadingRepository repository,
        IMemoryCache cache,
        ILogger<CachedMeterReadingRepository> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllCachedAsync(int cacheDuration = 60)
    {
        if (_cache.TryGetValue(AllMeterReadingsCacheKey, out IEnumerable<MeterReadingEntity> cachedReadings))
        {
            _logger.LogInformation("Показания счетчиков получены из кэша");
            return cachedReadings;
        }
        
        var readings = await _repository.GetAllAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
            .SetPriority(CacheItemPriority.Normal);
        
        _cache.Set(AllMeterReadingsCacheKey, readings, cacheOptions);
        _logger.LogInformation("Показания счетчиков добавлены в кэш");
        
        return readings;
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity?> GetByIdCachedAsync(Guid id, int cacheDuration = 60)
    {
        var cacheKey = $"{MeterReadingByIdCacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out MeterReadingEntity cachedReading))
        {
            _logger.LogInformation("Показание счетчика с ID {MeterReadingId} получено из кэша", id);
            return cachedReading;
        }
        
        var reading = await _repository.GetByIdAsync(id);
        
        if (reading != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
                .SetPriority(CacheItemPriority.Normal);
            
            _cache.Set(cacheKey, reading, cacheOptions);
            _logger.LogInformation("Показание счетчика с ID {MeterReadingId} добавлено в кэш", id);
        }
        
        return reading;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheAsync()
    {
        _cache.Remove(AllMeterReadingsCacheKey);
        _logger.LogInformation("Кэш всех показаний счетчиков инвалидирован");
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheForEntityAsync(Guid id)
    {
        var cacheKey = $"{MeterReadingByIdCacheKeyPrefix}{id}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Кэш показания счетчика с ID {MeterReadingId} инвалидирован", id);
        
        return Task.CompletedTask;
    }
} 