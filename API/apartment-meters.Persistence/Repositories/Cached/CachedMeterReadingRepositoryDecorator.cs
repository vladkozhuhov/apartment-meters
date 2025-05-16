using System.Text.Json;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories.Cached;

/// <summary>
/// Кэширующий репозиторий для работы с показаниями счетчиков
/// </summary>
public class CachedMeterReadingRepositoryDecorator : IMeterReadingRepository
{
    private readonly IMeterReadingRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CachedMeterReadingRepositoryDecorator> _logger;
    private readonly DistributedCacheEntryOptions _options;
    
    private const string MeterReadingByIdPrefix = "meter-reading-by-id-";
    private const string MeterReadingsByWaterMeterIdPrefix = "meter-readings-by-water-meter-id-";
    private const string LastMeterReadingByWaterMeterIdPrefix = "last-meter-reading-by-water-meter-id-";
    private const string AllMeterReadingsKey = "all-meter-readings";
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="decorated">Оригинальный репозиторий</param>
    /// <param name="distributedCache">Распределенный кэш</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedMeterReadingRepositoryDecorator(
        IMeterReadingRepository decorated, 
        IDistributedCache distributedCache,
        ILogger<CachedMeterReadingRepositoryDecorator> logger)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _logger = logger;
        
        // Настройка кэша (время жизни 10 минут)
        _options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity?> GetByIdAsync(Guid id)
    {
        string key = $"{MeterReadingByIdPrefix}{id}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var meterReading = JsonSerializer.Deserialize<MeterReadingEntity>(cachedData);
                _logger.LogInformation("Данные показания с ID {MeterReadingId} получены из кэша", id);
                return meterReading;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации показания из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetByIdAsync(id);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null)
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Данные показания с ID {MeterReadingId} сохранены в кэш", id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении показания в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllAsync()
    {
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(AllMeterReadingsKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var meterReadings = JsonSerializer.Deserialize<IEnumerable<MeterReadingEntity>>(cachedData);
                _logger.LogInformation("Данные всех показаний получены из кэша");
                return meterReadings ?? Enumerable.Empty<MeterReadingEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации списка показаний из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetAllAsync();
        
        // Сохраняем в кэш
        try
        {
            string serializedData = JsonSerializer.Serialize(result);
            await _distributedCache.SetStringAsync(AllMeterReadingsKey, serializedData, _options);
            _logger.LogInformation("Данные всех показаний сохранены в кэш");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при сохранении списка показаний в кэш");
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetByWaterMeterIdAsync(Guid waterMeterId)
    {
        string key = $"{MeterReadingsByWaterMeterIdPrefix}{waterMeterId}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var meterReadings = JsonSerializer.Deserialize<IEnumerable<MeterReadingEntity>>(cachedData);
                _logger.LogInformation("Данные показаний для счетчика с ID {WaterMeterId} получены из кэша", waterMeterId);
                return meterReadings ?? Enumerable.Empty<MeterReadingEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации списка показаний счетчика из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetByWaterMeterIdAsync(waterMeterId);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null && result.Any())
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Данные показаний для счетчика с ID {WaterMeterId} сохранены в кэш", waterMeterId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении списка показаний счетчика в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity?> GetLastByWaterMeterIdAsync(Guid waterMeterId)
    {
        string key = $"{LastMeterReadingByWaterMeterIdPrefix}{waterMeterId}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var meterReading = JsonSerializer.Deserialize<MeterReadingEntity>(cachedData);
                _logger.LogInformation("Последнее показание для счетчика с ID {WaterMeterId} получено из кэша", waterMeterId);
                return meterReading;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации последнего показания из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetLastByWaterMeterIdAsync(waterMeterId);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null)
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Последнее показание для счетчика с ID {WaterMeterId} сохранено в кэш", waterMeterId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении последнего показания в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity> AddAsync(MeterReadingEntity meterReading)
    {
        var result = await _decorated.AddAsync(meterReading);
        await InvalidateMeterReadingCacheAsync(meterReading.Id, meterReading.WaterMeterId);
        return result;
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(MeterReadingEntity meterReading)
    {
        var oldMeterReading = await _decorated.GetByIdAsync(meterReading.Id);
        await _decorated.UpdateAsync(meterReading);
        
        if (oldMeterReading != null)
        {
            // Если изменился счетчик, то инвалидируем кэш для обоих счетчиков
            if (oldMeterReading.WaterMeterId != meterReading.WaterMeterId)
            {
                await InvalidateMeterReadingCacheAsync(meterReading.Id, oldMeterReading.WaterMeterId);
            }
        }
        
        await InvalidateMeterReadingCacheAsync(meterReading.Id, meterReading.WaterMeterId);
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(MeterReadingEntity meterReading)
    {
        await _decorated.DeleteAsync(meterReading);
        await InvalidateMeterReadingCacheAsync(meterReading.Id, meterReading.WaterMeterId);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllByWaterMeterIdAsync(Guid waterMeterId)
    {
        string key = $"{MeterReadingsByWaterMeterIdPrefix}all-{waterMeterId}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var meterReadings = JsonSerializer.Deserialize<IEnumerable<MeterReadingEntity>>(cachedData);
                _logger.LogInformation("Все данные показаний для счетчика с ID {WaterMeterId} получены из кэша", waterMeterId);
                return meterReadings ?? Enumerable.Empty<MeterReadingEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации полного списка показаний счетчика из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetAllByWaterMeterIdAsync(waterMeterId);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null && result.Any())
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Все данные показаний для счетчика с ID {WaterMeterId} сохранены в кэш", waterMeterId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении полного списка показаний счетчика в кэш");
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Инвалидирует кэш для конкретного показания или для показаний счетчика
    /// </summary>
    /// <param name="meterReadingId">Идентификатор показания</param>
    /// <param name="waterMeterId">Идентификатор счетчика</param>
    private async Task InvalidateMeterReadingCacheAsync(Guid? meterReadingId = null, Guid? waterMeterId = null)
    {
        try
        {
            if (meterReadingId.HasValue)
            {
                string meterReadingKey = $"{MeterReadingByIdPrefix}{meterReadingId}";
                await _distributedCache.RemoveAsync(meterReadingKey);
                _logger.LogInformation("Кэш для показания с ID {MeterReadingId} очищен", meterReadingId);
            }
            
            if (waterMeterId.HasValue)
            {
                string waterMeterReadingsKey = $"{MeterReadingsByWaterMeterIdPrefix}{waterMeterId}";
                await _distributedCache.RemoveAsync(waterMeterReadingsKey);
                
                string allWaterMeterReadingsKey = $"{MeterReadingsByWaterMeterIdPrefix}all-{waterMeterId}";
                await _distributedCache.RemoveAsync(allWaterMeterReadingsKey);
                
                string lastWaterMeterReadingKey = $"{LastMeterReadingByWaterMeterIdPrefix}{waterMeterId}";
                await _distributedCache.RemoveAsync(lastWaterMeterReadingKey);
                
                _logger.LogInformation("Кэш для показаний счетчика с ID {WaterMeterId} очищен", waterMeterId);
            }
            
            // В любом случае инвалидируем список всех показаний
            await _distributedCache.RemoveAsync(AllMeterReadingsKey);
            _logger.LogInformation("Кэш для списка всех показаний очищен");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при очистке кэша показаний");
        }
    }
} 