using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с подписками на Push-уведомления
/// </summary>
public interface IPushSubscriptionRepository
{
    /// <summary>
    /// Получить все подписки для указанного пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список подписок пользователя</returns>
    Task<IEnumerable<PushSubscriptionEntity>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Получить подписку по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор подписки</param>
    /// <returns>Сущность подписки или null, если не найдена</returns>
    Task<PushSubscriptionEntity> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получить подписку по endpoint
    /// </summary>
    /// <param name="endpoint">Endpoint подписки</param>
    /// <returns>Сущность подписки или null, если не найдена</returns>
    Task<PushSubscriptionEntity> GetByEndpointAsync(string endpoint);
    
    /// <summary>
    /// Получить все активные подписки для всех пользователей
    /// </summary>
    /// <returns>Список всех активных подписок</returns>
    Task<IEnumerable<PushSubscriptionEntity>> GetAllActiveAsync();
    
    /// <summary>
    /// Добавить новую подписку
    /// </summary>
    /// <param name="subscription">Сущность подписки</param>
    /// <returns>Асинхронная задача</returns>
    Task AddAsync(PushSubscriptionEntity subscription);
    
    /// <summary>
    /// Обновить существующую подписку
    /// </summary>
    /// <param name="subscription">Сущность подписки с обновленными данными</param>
    /// <returns>Асинхронная задача</returns>
    Task UpdateAsync(PushSubscriptionEntity subscription);
    
    /// <summary>
    /// Удалить подписку
    /// </summary>
    /// <param name="id">Идентификатор подписки</param>
    /// <returns>Асинхронная задача</returns>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Удалить все подписки пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Асинхронная задача</returns>
    Task DeleteAllForUserAsync(Guid userId);
} 