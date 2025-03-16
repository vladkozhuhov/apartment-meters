using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Интерфейс для репозитория с поддержкой кэширования
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <typeparam name="TKey">Тип ключа</typeparam>
public interface ICachedRepository<TEntity, TKey> where TEntity : class
{
    /// <summary>
    /// Получить все сущности с поддержкой кэширования
    /// </summary>
    /// <param name="cacheDuration">Продолжительность кэширования в секундах (по умолчанию 60)</param>
    /// <returns>Список сущностей</returns>
    Task<IEnumerable<TEntity>> GetAllCachedAsync(int cacheDuration = 60);
    
    /// <summary>
    /// Получить сущность по идентификатору с поддержкой кэширования
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cacheDuration">Продолжительность кэширования в секундах (по умолчанию 60)</param>
    /// <returns>Сущность или null, если не найдена</returns>
    Task<TEntity?> GetByIdCachedAsync(TKey id, int cacheDuration = 60);
    
    /// <summary>
    /// Инвалидировать кэш для всех сущностей
    /// </summary>
    Task InvalidateCacheAsync();
    
    /// <summary>
    /// Инвалидировать кэш для конкретной сущности
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    Task InvalidateCacheForEntityAsync(TKey id);
} 