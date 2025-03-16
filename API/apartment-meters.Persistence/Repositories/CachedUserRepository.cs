using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Repositories;

namespace Persistence.Repositories;

/// <summary>
/// Репозиторий для пользователей с поддержкой кэширования
/// </summary>
public class CachedUserRepository : ICachedRepository<UserEntity, Guid>
{
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedUserRepository> _logger;
    
    private const string AllUsersCacheKey = "all_users";
    private static readonly string UserByIdCacheKeyPrefix = "user_";
    
    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="repository">Базовый репозиторий пользователей</param>
    /// <param name="cache">Сервис кэширования</param>
    /// <param name="logger">Сервис логирования</param>
    public CachedUserRepository(
        IUserRepository repository,
        IMemoryCache cache,
        ILogger<CachedUserRepository> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<UserEntity>> GetAllCachedAsync(int cacheDuration = 60)
    {
        if (_cache.TryGetValue(AllUsersCacheKey, out IEnumerable<UserEntity> cachedUsers))
        {
            _logger.LogInformation("Пользователи получены из кэша");
            return cachedUsers;
        }
        
        var users = await _repository.GetAllAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
            .SetPriority(CacheItemPriority.Normal);
        
        _cache.Set(AllUsersCacheKey, users, cacheOptions);
        _logger.LogInformation("Пользователи добавлены в кэш");
        
        return users;
    }
    
    /// <inheritdoc />
    public async Task<UserEntity?> GetByIdCachedAsync(Guid id, int cacheDuration = 60)
    {
        var cacheKey = $"{UserByIdCacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out UserEntity cachedUser))
        {
            _logger.LogInformation("Пользователь с ID {UserId} получен из кэша", id);
            return cachedUser;
        }
        
        var user = await _repository.GetByIdAsync(id);
        
        if (user != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration))
                .SetPriority(CacheItemPriority.Normal);
            
            _cache.Set(cacheKey, user, cacheOptions);
            _logger.LogInformation("Пользователь с ID {UserId} добавлен в кэш", id);
        }
        
        return user;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheAsync()
    {
        _cache.Remove(AllUsersCacheKey);
        _logger.LogInformation("Кэш всех пользователей инвалидирован");
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheForEntityAsync(Guid id)
    {
        var cacheKey = $"{UserByIdCacheKeyPrefix}{id}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Кэш пользователя с ID {UserId} инвалидирован", id);
        
        return Task.CompletedTask;
    }
} 