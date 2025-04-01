using System;

namespace Domain.Entities;

/// <summary>
/// Сущность подписки на Push-уведомления
/// </summary>
public class PushSubscriptionEntity
{
    /// <summary>
    /// Уникальный идентификатор подписки
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя, которому принадлежит подписка
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Endpoint для отправки push-уведомлений
    /// </summary>
    public string Endpoint { get; set; }
    
    /// <summary>
    /// Ключ P256DH для шифрования данных
    /// </summary>
    public string P256dh { get; set; }
    
    /// <summary>
    /// Ключ аутентификации
    /// </summary>
    public string Auth { get; set; }
    
    /// <summary>
    /// Дата создания подписки
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего отправленного уведомления
    /// </summary>
    public DateTime? LastNotificationAt { get; set; }
    
    /// <summary>
    /// Тип устройства, на котором зарегистрирована подписка
    /// </summary>
    public string DeviceType { get; set; }
    
    /// <summary>
    /// Навигационное свойство для связи с пользователем
    /// </summary>
    public UserEntity User { get; set; }
} 