namespace Application.Models.NotificationModel;

/// <summary>
/// Настройки для Web Push уведомлений
/// </summary>
public class PushNotificationSettings
{
    /// <summary>
    /// Тема уведомлений (обычно mailto:адрес)
    /// </summary>
    public string Subject { get; set; }
    
    /// <summary>
    /// Публичный ключ VAPID
    /// </summary>
    public string PublicKey { get; set; }
    
    /// <summary>
    /// Приватный ключ VAPID
    /// </summary>
    public string PrivateKey { get; set; }
} 