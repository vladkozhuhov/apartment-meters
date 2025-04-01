using System;

namespace Application.Models.NotificationModel;

/// <summary>
/// DTO для передачи данных о подписке на Push-уведомления
/// </summary>
public class PushSubscriptionDto
{
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
    /// Тип устройства (опционально)
    /// </summary>
    public string DeviceType { get; set; }
} 