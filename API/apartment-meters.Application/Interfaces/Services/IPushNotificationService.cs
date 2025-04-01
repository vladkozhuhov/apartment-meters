using System;
using System.Threading.Tasks;
using Application.Models.NotificationModel;

namespace Application.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для работы с Push-уведомлениями
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Получить публичный VAPID ключ для Web Push
    /// </summary>
    /// <returns>Публичный ключ в base64 формате</returns>
    string GetPublicKey();
    
    /// <summary>
    /// Сохранить подписку пользователя на Push-уведомления
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="subscription">Данные подписки</param>
    /// <returns>Асинхронная задача</returns>
    Task SaveSubscriptionAsync(Guid userId, PushSubscriptionDto subscription);
    
    /// <summary>
    /// Удалить подписку пользователя на Push-уведомления
    /// </summary>
    /// <param name="endpoint">Endpoint подписки</param>
    /// <returns>Асинхронная задача</returns>
    Task DeleteSubscriptionAsync(string endpoint);
    
    /// <summary>
    /// Удалить все подписки пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Асинхронная задача</returns>
    Task DeleteAllSubscriptionsAsync(Guid userId);
    
    /// <summary>
    /// Отправить уведомление всем пользователям о необходимости передать показания счетчиков
    /// </summary>
    /// <returns>Количество успешно отправленных уведомлений</returns>
    Task<int> SendMeterReadingReminderToAllAsync();
    
    /// <summary>
    /// Отправить уведомление конкретному пользователю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="payload">Полезная нагрузка уведомления</param>
    /// <returns>Количество успешно отправленных уведомлений</returns>
    Task<int> SendNotificationToUserAsync(Guid userId, PushNotificationPayload payload);
} 