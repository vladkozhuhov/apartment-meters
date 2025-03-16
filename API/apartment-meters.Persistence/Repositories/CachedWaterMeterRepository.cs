using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Repositories;

namespace Persistence.Repositories;

/// <summary>
/// Репозиторий для счетчиков воды с поддержкой кэширования
/// </summary>
public class CachedWaterMeterRepository : ICachedRepository<WaterMeterEntity, Guid>
{
    private readonly IWaterMeterRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedWaterMeterRepository> _logger;
    
    private const string AllWaterMetersCacheKey = "all_water_meters";
    private static readonly string WaterMeterByIdCacheKeyPrefix = "water_meter_";
    private static readonly string WaterMetersByUserIdPrefix = "water_meters_user_";
    
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="repository">Базовый репозиторий счетчиков воды</param>
    /// <param name="cache">Сервис кэширования</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedWaterMeterRepository(
        IWaterMeterRepository repository,
        IMemoryCache cache,
        ILogger<CachedWaterMeterRepository> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<IEnumerable<WaterMeterEntity>> GetAllCachedAsync(int cacheDuration = 60)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<WaterMeterEntity?> GetByIdCachedAsync(Guid id, int cacheDuration = 60)
    {
        var cacheKey = $"{WaterMeterByIdCacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out WaterMeterEntity cachedWaterMeter))
        {
            _logger.LogInformation("Счетчик воды с ID {WaterMeterId} получен из кэша", id);
            return cachedWaterMeter;
        }
        
        var waterMeter = await _repository.GetByIdAsync(id);
        
        if (waterMeter != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
                .SetPriority(CacheItemPriority.Normal);
            
            _cache.Set(cacheKey, waterMeter, cacheOptions);
            _logger.LogInformation("Счетчик воды с ID {WaterMeterId} добавлен в кэш", id);
        }
        
        return waterMeter;
    }
    
    /// <summary>
    /// Получить счетчики пользователя с использованием кэша
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cacheDuration">Продолжительность кэширования в секундах</param>
    /// <returns>Список счетчиков пользователя</returns>
    public async Task<IEnumerable<WaterMeterEntity>> GetByUserIdCachedAsync(Guid userId, int cacheDuration = 60)
    {
        var cacheKey = $"{WaterMetersByUserIdPrefix}{userId}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<WaterMeterEntity> cachedWaterMeters))
        {
            _logger.LogInformation("Счетчики воды пользователя с ID {UserId} получены из кэша", userId);
            return cachedWaterMeters;
        }
        
        var waterMeters = await _repository.GetByUserIdAsync(userId);
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
            .SetPriority(CacheItemPriority.Normal);
        
        _cache.Set(cacheKey, waterMeters, cacheOptions);
        _logger.LogInformation("Счетчики воды пользователя с ID {UserId} добавлены в кэш", userId);
        
        return waterMeters;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheAsync()
    {
        _cache.Remove(AllWaterMetersCacheKey);
        _logger.LogInformation("Кэш всех счетчиков воды инвалидирован");
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheForEntityAsync(Guid id)
    {
        var cacheKey = $"{WaterMeterByIdCacheKeyPrefix}{id}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Кэш счетчика воды с ID {WaterMeterId} инвалидирован", id);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Инвалидировать кэш для счетчиков пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    public Task InvalidateCacheForUserAsync(Guid userId)
    {
        var cacheKey = $"{WaterMetersByUserIdPrefix}{userId}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Кэш счетчиков воды пользователя с ID {UserId} инвалидирован", userId);
        
        return Task.CompletedTask;
    }
} 