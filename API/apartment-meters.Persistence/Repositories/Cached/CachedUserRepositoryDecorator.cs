using System.Text.Json;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories.Cached;

/// <summary>
/// Кэширующий репозиторий для работы с пользователями
/// </summary>
public class CachedUserRepositoryDecorator : IUserRepository
{
    private readonly IUserRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CachedUserRepositoryDecorator> _logger;
    private readonly DistributedCacheEntryOptions _options;
    
    private const string UserByIdPrefix = "user-by-id-";
    private const string UserByApartmentNumberPrefix = "user-by-apartment-";
    private const string AllUsersKey = "all-users";
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="decorated">Оригинальный репозиторий</param>
    /// <param name="distributedCache">Распределенный кэш</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedUserRepositoryDecorator(
        IUserRepository decorated, 
        IDistributedCache distributedCache,
        ILogger<CachedUserRepositoryDecorator> logger)
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
    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        string key = $"{UserByIdPrefix}{id}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserEntity>(cachedData);
                _logger.LogInformation("Данные пользователя с ID {UserId} получены из кэша", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации пользователя из кэша");
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
                _logger.LogInformation("Данные пользователя с ID {UserId} сохранены в кэш", id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении пользователя в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<UserEntity?> GetByApartmentNumberAsync(int apartmentNumber)
    {
        string key = $"{UserByApartmentNumberPrefix}{apartmentNumber}";
        
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(key);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserEntity>(cachedData);
                _logger.LogInformation("Данные пользователя с номером квартиры {ApartmentNumber} получены из кэша", apartmentNumber);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации пользователя из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetByApartmentNumberAsync(apartmentNumber);
        
        // Если данные найдены, сохраняем в кэш
        if (result != null)
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(key, serializedData, _options);
                _logger.LogInformation("Данные пользователя с номером квартиры {ApartmentNumber} сохранены в кэш", apartmentNumber);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при сохранении пользователя в кэш");
            }
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        // Пытаемся получить данные из кэша
        string? cachedData = await _distributedCache.GetStringAsync(AllUsersKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var users = JsonSerializer.Deserialize<IEnumerable<UserEntity>>(cachedData);
                _logger.LogInformation("Данные всех пользователей получены из кэша");
                return users ?? Enumerable.Empty<UserEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при десериализации списка пользователей из кэша");
            }
        }
        
        // Если данных нет в кэше, получаем из репозитория
        var result = await _decorated.GetAllAsync();
        
        // Сохраняем в кэш
        try
        {
            string serializedData = JsonSerializer.Serialize(result);
            await _distributedCache.SetStringAsync(AllUsersKey, serializedData, _options);
            _logger.LogInformation("Данные всех пользователей сохранены в кэш");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при сохранении списка пользователей в кэш");
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<UserEntity> AddAsync(UserEntity user)
    {
        var result = await _decorated.AddAsync(user);
        await InvalidateUserCacheAsync(user.Id);
        await InvalidateUserCacheByApartmentNumberAsync(user.ApartmentNumber);
        return result;
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(UserEntity user)
    {
        var oldUser = await _decorated.GetByIdAsync(user.Id);
        await _decorated.UpdateAsync(user);
        
        // Инвалидируем кэш по ID
        await InvalidateUserCacheAsync(user.Id);
        
        // Если изменился номер квартиры, инвалидируем кэш и по старому, и по новому номеру
        if (oldUser != null && oldUser.ApartmentNumber != user.ApartmentNumber)
        {
            await InvalidateUserCacheByApartmentNumberAsync(oldUser.ApartmentNumber);
        }
        
        await InvalidateUserCacheByApartmentNumberAsync(user.ApartmentNumber);
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(UserEntity user)
    {
        await _decorated.DeleteAsync(user);
        await InvalidateUserCacheAsync(user.Id);
        await InvalidateUserCacheByApartmentNumberAsync(user.ApartmentNumber);
    }
    
    /// <inheritdoc />
    public async Task UpdatePhoneAsync(Guid userId, string phoneNumber)
    {
        // Вызываем метод базового репозитория
        await _decorated.UpdatePhoneAsync(userId, phoneNumber);
        
        // Инвалидируем кэш для данного пользователя
        await InvalidateUserCacheAsync(userId);
        
        // Инвалидируем общий кэш всех пользователей
        await _distributedCache.RemoveAsync(AllUsersKey);
        
        _logger.LogInformation("Номер телефона для пользователя с ID {UserId} обновлен, кэш очищен", userId);
    }
    
    /// <summary>
    /// Инвалидирует кэш для конкретного пользователя или для всех пользователей
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (если null, то инвалидируется весь кэш)</param>
    private async Task InvalidateUserCacheAsync(Guid? userId = null)
    {
        try
        {
            if (userId.HasValue)
            {
                string userKey = $"{UserByIdPrefix}{userId}";
                await _distributedCache.RemoveAsync(userKey);
                _logger.LogInformation("Кэш для пользователя с ID {UserId} очищен", userId);
            }
            
            // В любом случае инвалидируем список всех пользователей
            await _distributedCache.RemoveAsync(AllUsersKey);
            _logger.LogInformation("Кэш для списка всех пользователей очищен");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при очистке кэша пользователей");
        }
    }
    
    /// <summary>
    /// Инвалидирует кэш для пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    private async Task InvalidateUserCacheByApartmentNumberAsync(int apartmentNumber)
    {
        try
        {
            string userKey = $"{UserByApartmentNumberPrefix}{apartmentNumber}";
            await _distributedCache.RemoveAsync(userKey);
            _logger.LogInformation("Кэш для пользователя с номером квартиры {ApartmentNumber} очищен", apartmentNumber);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при очистке кэша пользователя по номеру квартиры");
        }
    }
} 