using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Models.NotificationModel;

/// <summary>
/// Полезная нагрузка для push-уведомления
/// </summary>
public class PushNotificationPayload
{
    /// <summary>
    /// Заголовок уведомления
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    /// <summary>
    /// Основной текст уведомления
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; }
    
    /// <summary>
    /// URL иконки уведомления
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    
    /// <summary>
    /// URL изображения для уведомления
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; set; }
    
    /// <summary>
    /// URL, на который нужно перейти при клике на уведомление
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    /// <summary>
    /// Произвольные данные, которые будут переданы в service worker
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; }
    
    /// <summary>
    /// Список действий для уведомления
    /// </summary>
    [JsonPropertyName("actions")]
    public List<PushNotificationAction> Actions { get; set; }
    
    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public PushNotificationPayload()
    {
        Data = new Dictionary<string, object>();
        Actions = new List<PushNotificationAction>();
    }
}

/// <summary>
/// Действие для push-уведомления
/// </summary>
public class PushNotificationAction
{
    /// <summary>
    /// Идентификатор действия
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; }
    
    /// <summary>
    /// Название действия
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    /// <summary>
    /// URL иконки действия
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
} 