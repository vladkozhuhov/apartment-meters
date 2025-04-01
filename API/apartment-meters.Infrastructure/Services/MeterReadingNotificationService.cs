using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Фоновая служба для отправки уведомлений о необходимости передачи показаний счетчиков
/// </summary>
public class MeterReadingNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MeterReadingNotificationService> _logger;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serviceProvider">Провайдер служб</param>
    /// <param name="logger">Логгер</param>
    public MeterReadingNotificationService(
        IServiceProvider serviceProvider,
        ILogger<MeterReadingNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Метод выполнения фоновой службы
    /// </summary>
    /// <param name="stoppingToken">Токен отмены</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Служба уведомлений о показаниях счетчиков запущена");

        // Запускаем рабочий цикл
        await ProcessNotificationsAsync(stoppingToken);
    }

    /// <summary>
    /// Основной рабочий цикл службы
    /// </summary>
    private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                
                // Отправляем уведомления 23-го числа каждого месяца
                if (now.Day == 23)
                {
                    _logger.LogInformation("Наступило 23-е число. Отправляем уведомления о необходимости передачи показаний");
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var pushNotificationService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();
                        
                        // Отправляем уведомления о необходимости передачи показаний
                        var sentCount = await pushNotificationService.SendMeterReadingReminderToAllAsync();
                        
                        _logger.LogInformation("Отправлено {SentCount} уведомлений о необходимости передачи показаний", sentCount);
                    }
                    
                    // Ждем до следующего дня, чтобы не отправлять уведомления повторно
                    var tomorrow = now.AddDays(1).Date;
                    var timeUntilTomorrow = tomorrow - now;
                    
                    await Task.Delay(timeUntilTomorrow, stoppingToken);
                }
                else
                {
                    // Каждый час проверяем, не пора ли отправлять уведомления
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Ожидаемое исключение при остановке службы
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке уведомлений о необходимости передачи показаний");
                
                // В случае ошибки ждем 15 минут перед повторной попыткой
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
        
        _logger.LogInformation("Служба уведомлений о показаниях счетчиков остановлена");
    }
} 