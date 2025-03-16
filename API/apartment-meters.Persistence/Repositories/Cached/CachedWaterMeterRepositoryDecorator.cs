using System.Text.Json;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories.Cached;

/// <summary>
/// Кэширующий репозиторий для работы со счетчиками воды
/// </summary>
public class CachedWaterMeterRepositoryDecorator : IWaterMeterRepository
{
    private readonly IWaterMeterRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CachedWaterMeterRepositoryDecorator> _logger;
    private readonly DistributedCacheEntryOptions _options;
    
    private const string WaterMeterByIdPrefix = "water-meter-by-id-";
    private const string WaterMetersByUserIdPrefix = "water-meters-by-user-id-";
    private const string AllWaterMetersKey = "all-water-meters";
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="decorated">Оригинальный репозиторий</param>
    /// <param name="distributedCache">Распределенный кэш</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedWaterMeterRepositoryDecorator(
        IWaterMeterRepository decorated, 
        IDistributedCache distributedCache,
        ILogger<CachedWaterMeterRepositoryDecorator> logger)
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
    public async Task<WaterMeterEntity?> GetByIdAsync(Guid id)
    {
        string key = $"{WaterMeterByIdPrefix}{id}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var waterMeter = JsonSerializer.Deserialize<WaterMeterEntity>(cachedData);
                _logger.LogInformation("Данные счетчика с ID {WaterMeterId} получены из кэша", id);
                return waterMeter;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации счетчика из кэша");
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
                _logger.LogInformation("Данные счетчика с ID {WaterMeterId} сохранены в кэш", id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении счетчика в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<WaterMeterEntity>> GetByUserIdAsync(Guid userId)
    {
        string key = $"{WaterMetersByUserIdPrefix}{userId}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var waterMeters = JsonSerializer.Deserialize<IEnumerable<WaterMeterEntity>>(cachedData);
                _logger.LogInformation("Данные счетчиков пользователя с ID {UserId} получены из кэша", userId);
                return waterMeters ?? Enumerable.Empty<WaterMeterEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации списка счетчиков пользователя из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetByUserIdAsync(userId);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null && result.Any())
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Данные счетчиков пользователя с ID {UserId} сохранены в кэш", userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении списка счетчиков пользователя в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<WaterMeterEntity> AddAsync(WaterMeterEntity waterMeter)
    {
        var result = await _decorated.AddAsync(waterMeter);
        await InvalidateWaterMeterCacheAsync(waterMeter.Id, waterMeter.UserId);
        return result;
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(WaterMeterEntity waterMeter)
    {
        var oldWaterMeter = await _decorated.GetByIdAsync(waterMeter.Id);
        await _decorated.UpdateAsync(waterMeter);
        
        if (oldWaterMeter != null)
        {
            // Если изменился пользователь, то инвалидируем кэш для обоих пользователей
            if (oldWaterMeter.UserId != waterMeter.UserId)
            {
                await InvalidateWaterMeterCacheAsync(waterMeter.Id, oldWaterMeter.UserId);
            }
        }
        
        await InvalidateWaterMeterCacheAsync(waterMeter.Id, waterMeter.UserId);
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(WaterMeterEntity waterMeter)
    {
        await _decorated.DeleteAsync(waterMeter);
        await InvalidateWaterMeterCacheAsync(waterMeter.Id, waterMeter.UserId);
    }
    
    /// <summary>
    /// Инвалидирует кэш для конкретного счетчика или для всех счетчиков
    /// </summary>
    /// <param name="waterMeterId">Идентификатор счетчика</param>
    /// <param name="userId">Идентификатор пользователя</param>
    private async Task InvalidateWaterMeterCacheAsync(Guid? waterMeterId = null, Guid? userId = null)
    {
        try
        {
            if (waterMeterId.HasValue)
            {
                string waterMeterKey = $"{WaterMeterByIdPrefix}{waterMeterId}";
                await _distributedCache.RemoveAsync(waterMeterKey);
                _logger.LogInformation("Кэш для счетчика с ID {WaterMeterId} очищен", waterMeterId);
            }
            
            if (userId.HasValue)
            {
                string userWaterMetersKey = $"{WaterMetersByUserIdPrefix}{userId}";
                await _distributedCache.RemoveAsync(userWaterMetersKey);
                _logger.LogInformation("Кэш для счетчиков пользователя с ID {UserId} очищен", userId);
            }
            
            // В любом случае инвалидируем список всех счетчиков
            await _distributedCache.RemoveAsync(AllWaterMetersKey);
            _logger.LogInformation("Кэш для списка всех счетчиков очищен");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при очистке кэша счетчиков");
        }
    }
} 