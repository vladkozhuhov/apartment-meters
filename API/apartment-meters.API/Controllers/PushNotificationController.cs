using System.Security.Claims;
using Application.Interfaces.Services;
using Application.Models.NotificationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с Web Push уведомлениями
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Требуем аутентификацию для всех методов
public class PushNotificationController : ControllerBase
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<PushNotificationController> _logger;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="pushNotificationService">Сервис отправки уведомлений</param>
    /// <param name="logger">Логгер</param>
    public PushNotificationController(
        IPushNotificationService pushNotificationService,
        ILogger<PushNotificationController> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    /// <summary>
    /// Получает публичный VAPID ключ для настройки push-уведомлений
    /// </summary>
    /// <returns>Публичный VAPID ключ</returns>
    [HttpGet("vapid-public-key")]
    public ActionResult<string> GetVapidPublicKey()
    {
        var publicKey = _pushNotificationService.GetPublicKey();
        return Ok(publicKey);
    }

    /// <summary>
    /// Сохраняет подписку на push-уведомления
    /// </summary>
    /// <param name="subscription">Данные подписки</param>
    /// <returns>Результат операции</returns>
    [HttpPost("subscriptions")]
    public async Task<IActionResult> SaveSubscription([FromBody] PushSubscriptionDto subscription)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Получаем идентификатор пользователя из токена
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Не удалось определить идентификатор пользователя");
            }

            await _pushNotificationService.SaveSubscriptionAsync(userId, subscription);
            return Ok(new { success = true, message = "Подписка на уведомления успешно сохранена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении подписки на push-уведомления");
            return StatusCode(500, new { success = false, message = "Ошибка сервера при сохранении подписки" });
        }
    }

    /// <summary>
    /// Удаляет подписку на push-уведомления
    /// </summary>
    /// <param name="endpoint">Endpoint подписки</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("subscriptions")]
    public async Task<IActionResult> DeleteSubscription([FromQuery] string endpoint)
    {
        if (string.IsNullOrEmpty(endpoint))
        {
            return BadRequest("Endpoint не указан");
        }

        try
        {
            await _pushNotificationService.DeleteSubscriptionAsync(endpoint);
            return Ok(new { success = true, message = "Подписка на уведомления успешно удалена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении подписки на push-уведомления");
            return StatusCode(500, new { success = false, message = "Ошибка сервера при удалении подписки" });
        }
    }

    /// <summary>
    /// Отправляет тестовое уведомление текущему пользователю
    /// </summary>
    /// <returns>Результат операции</returns>
    [HttpPost("test")]
    public async Task<IActionResult> SendTestNotification()
    {
        try
        {
            // Получаем идентификатор пользователя из токена
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Не удалось определить идентификатор пользователя");
            }

            var payload = new PushNotificationPayload
            {
                Title = "Тестовое уведомление",
                Body = "Это тестовое push-уведомление от сервиса учета показаний.",
                Icon = "/icons/logo-192.png",
                Url = "/dashboard",
                Data = new Dictionary<string, object>
                {
                    { "type", "test-notification" },
                    { "timestamp", DateTime.UtcNow.ToString("o") }
                }
            };

            var sentCount = await _pushNotificationService.SendNotificationToUserAsync(userId, payload);
            
            if (sentCount > 0)
            {
                return Ok(new { success = true, message = $"Отправлено {sentCount} тестовых уведомлений" });
            }
            else
            {
                return Ok(new { success = false, message = "У вас нет активных подписок на push-уведомления" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке тестового push-уведомления");
            return StatusCode(500, new { success = false, message = "Ошибка сервера при отправке тестового уведомления" });
        }
    }

    /// <summary>
    /// Получает идентификатор пользователя из токена
    /// </summary>
    /// <returns>Идентификатор пользователя</returns>
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return userId;
        }
        
        return Guid.Empty;
    }
} 