using System.Text.Json;
using Application.Interfaces.Services;
using Application.Models.NotificationModel;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;

namespace Infrastructure.Services;

/// <summary>
/// Сервис для работы с Web Push уведомлениями
/// </summary>
public class PushNotificationService : IPushNotificationService
{
    private readonly IPushSubscriptionRepository _subscriptionRepository;
    private readonly ILogger<PushNotificationService> _logger;
    private readonly PushNotificationSettings _settings;
    private readonly WebPushClient _pushClient;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="subscriptionRepository">Репозиторий для подписок на уведомления</param>
    /// <param name="settings">Настройки для отправки уведомлений</param>
    /// <param name="logger">Логгер</param>
    public PushNotificationService(
        IPushSubscriptionRepository subscriptionRepository,
        IOptions<PushNotificationSettings> settings,
        ILogger<PushNotificationService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
        _settings = settings.Value;
        _pushClient = new WebPushClient();
    }

    /// <inheritdoc />
    public string GetPublicKey()
    {
        return _settings.PublicKey;
    }

    /// <inheritdoc />
    public async Task SaveSubscriptionAsync(Guid userId, PushSubscriptionDto subscription)
    {
        try
        {
            // Проверяем, существует ли уже подписка с таким endpoint
            var existingSubscription = await _subscriptionRepository.GetByEndpointAsync(subscription.Endpoint);

            if (existingSubscription != null)
            {
                // Если подписка существует, но для другого пользователя, 
                // это может быть случай, когда устройство используется разными пользователями
                if (existingSubscription.UserId != userId)
                {
                    _logger.LogWarning("Подписка с endpoint {Endpoint} уже существует для пользователя {ExistingUserId}, но запрошена для {UserId}. Обновляем.", 
                        subscription.Endpoint, existingSubscription.UserId, userId);
                    existingSubscription.UserId = userId;
                }

                // Обновляем данные подписки
                existingSubscription.P256dh = subscription.P256dh;
                existingSubscription.Auth = subscription.Auth;
                existingSubscription.DeviceType = subscription.DeviceType;

                await _subscriptionRepository.UpdateAsync(existingSubscription);
                _logger.LogInformation("Обновлена существующая подписка на push-уведомления для пользователя {UserId}", userId);
            }
            else
            {
                // Создаем новую подписку
                var newSubscription = new PushSubscriptionEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Endpoint = subscription.Endpoint,
                    P256dh = subscription.P256dh,
                    Auth = subscription.Auth,
                    DeviceType = subscription.DeviceType,
                    CreatedAt = DateTime.UtcNow,
                    LastNotificationAt = null
                };

                await _subscriptionRepository.AddAsync(newSubscription);
                _logger.LogInformation("Добавлена новая подписка на push-уведомления для пользователя {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении подписки на push-уведомления для пользователя {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteSubscriptionAsync(string endpoint)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByEndpointAsync(endpoint);
            if (subscription != null)
            {
                await _subscriptionRepository.DeleteAsync(subscription.Id);
                _logger.LogInformation("Удалена подписка на push-уведомления с endpoint {Endpoint}", endpoint);
            }
            else
            {
                _logger.LogWarning("Попытка удалить несуществующую подписку с endpoint {Endpoint}", endpoint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении подписки на push-уведомления с endpoint {Endpoint}", endpoint);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllSubscriptionsAsync(Guid userId)
    {
        try
        {
            await _subscriptionRepository.DeleteAllForUserAsync(userId);
            _logger.LogInformation("Удалены все подписки на push-уведомления для пользователя {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении всех подписок на push-уведомления для пользователя {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> SendMeterReadingReminderToAllAsync()
    {
        var payload = new PushNotificationPayload
        {
            Title = "Напоминание о показаниях",
            Body = "Пожалуйста, не забудьте передать показания счетчиков воды до конца месяца.",
            Icon = "/icons/logo-192.png",
            Url = "/meters",
            Data = new Dictionary<string, object>
            {
                { "type", "meter-reading-reminder" },
                { "timestamp", DateTime.UtcNow.ToString("o") }
            }
        };

        return await SendNotificationToAllAsync(payload);
    }

    /// <inheritdoc />
    public async Task<int> SendNotificationToUserAsync(Guid userId, PushNotificationPayload payload)
    {
        int sentCount = 0;

        try
        {
            var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
            if (!subscriptions.Any())
            {
                _logger.LogInformation("Пользователь {UserId} не имеет подписок на push-уведомления", userId);
                return 0;
            }

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await SendNotificationToSubscriptionAsync(subscription, payload);
                    sentCount++;
                }
                catch (WebPushException ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке push-уведомления для пользователя {UserId} на endpoint {Endpoint}", 
                        userId, subscription.Endpoint);
                        
                    // Если сервер отправки возвращает ошибку 410 (Gone), значит подписка больше не действительна
                    if (ex.StatusCode == System.Net.HttpStatusCode.Gone)
                    {
                        _logger.LogWarning("Подписка больше не действительна (410 Gone). Удаляем подписку для пользователя {UserId}, endpoint {Endpoint}",
                            userId, subscription.Endpoint);
                        await _subscriptionRepository.DeleteAsync(subscription.Id);
                    }
                }
            }

            _logger.LogInformation("Отправлено {SentCount} push-уведомлений пользователю {UserId}", sentCount, userId);
            return sentCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке push-уведомлений пользователю {UserId}", userId);
            throw;
        }
    }
    
    /// <summary>
    /// Отправляет уведомление всем пользователям
    /// </summary>
    /// <param name="payload">Данные уведомления</param>
    /// <returns>Количество отправленных уведомлений</returns>
    private async Task<int> SendNotificationToAllAsync(PushNotificationPayload payload)
    {
        int sentCount = 0;
        int failedCount = 0;
        int deletedCount = 0;

        try
        {
            var subscriptions = await _subscriptionRepository.GetAllActiveAsync();
            if (!subscriptions.Any())
            {
                _logger.LogInformation("Нет активных подписок на push-уведомления");
                return 0;
            }

            _logger.LogInformation("Начинаем отправку push-уведомлений для {SubscriptionCount} подписок", subscriptions.Count());

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await SendNotificationToSubscriptionAsync(subscription, payload);
                    sentCount++;
                    
                    // Обновляем время последней отправки уведомления
                    subscription.LastNotificationAt = DateTime.UtcNow;
                    await _subscriptionRepository.UpdateAsync(subscription);
                }
                catch (WebPushException ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Ошибка при отправке push-уведомления для пользователя {UserId} на endpoint {Endpoint}", 
                        subscription.UserId, subscription.Endpoint);
                        
                    // Если сервер отправки возвращает ошибку 410 (Gone), значит подписка больше не действительна
                    if (ex.StatusCode == System.Net.HttpStatusCode.Gone)
                    {
                        deletedCount++;
                        _logger.LogWarning("Подписка больше не действительна (410 Gone). Удаляем подписку для пользователя {UserId}, endpoint {Endpoint}",
                            subscription.UserId, subscription.Endpoint);
                        await _subscriptionRepository.DeleteAsync(subscription.Id);
                    }
                }
            }

            _logger.LogInformation("Результаты отправки push-уведомлений: отправлено {SentCount}, не доставлено {FailedCount}, удалено {DeletedCount}", 
                sentCount, failedCount, deletedCount);
                
            return sentCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при массовой отправке push-уведомлений");
            throw;
        }
    }

    /// <summary>
    /// Отправляет уведомление на конкретную подписку
    /// </summary>
    /// <param name="subscription">Данные подписки</param>
    /// <param name="payload">Данные уведомления</param>
    private async Task SendNotificationToSubscriptionAsync(PushSubscriptionEntity subscription, PushNotificationPayload payload)
    {
        var pushSubscription = new PushSubscription(
            subscription.Endpoint,
            subscription.P256dh,
            subscription.Auth
        );
        
        var vapidDetails = new VapidDetails(
            _settings.Subject,
            _settings.PublicKey,
            _settings.PrivateKey
        );
        
        var options = new Dictionary<string, object>
        {
            { "vapidDetails", vapidDetails },
            { "urgency", "normal" }
        };
        
        string json = JsonSerializer.Serialize(payload);
        await _pushClient.SendNotificationAsync(pushSubscription, json, options);
    }
} 