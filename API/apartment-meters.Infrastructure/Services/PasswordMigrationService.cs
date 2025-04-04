using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Infrastructure.Services;

/// <summary>
/// Сервис для миграции паролей пользователей из текстового формата в хешированный (BCrypt)
/// </summary>
public class PasswordMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PasswordMigrationService> _logger;

    public PasswordMigrationService(
        IServiceProvider serviceProvider, 
        ILogger<PasswordMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Запускает миграцию паролей пользователей
    /// </summary>
    public async Task MigratePasswords()
    {
        try
        {
            _logger.LogInformation("Начало миграции паролей пользователей");

            // Создаем новый скоуп для DbContext, чтобы не смешивать его с основным жизненным циклом приложения
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Получаем всех пользователей
            var users = await dbContext.Users.ToListAsync();
            int migratedCount = 0;

            foreach (var user in users)
            {
                // Проверяем, не хеширован ли уже пароль
                if (!user.Password.StartsWith("$2a$") && 
                    !user.Password.StartsWith("$2b$") && 
                    !user.Password.StartsWith("$2y$"))
                {
                    // Сохраняем оригинальный пароль для проверки
                    string originalPassword = user.Password;
                    
                    // Хешируем пароль
                    user.Password = BCrypt.Net.BCrypt.HashPassword(originalPassword, workFactor: 12);
                    migratedCount++;
                    
                    _logger.LogInformation("Пароль пользователя с ID {UserId} успешно хеширован", user.Id);
                }
            }

            // Сохраняем изменения в базе данных
            if (migratedCount > 0)
            {
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Миграция паролей завершена. Обновлено {Count} паролей", migratedCount);
            }
            else
            {
                _logger.LogInformation("Миграция паролей завершена. Все пароли уже в хешированном формате");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при миграции паролей пользователей");
        }
    }
} 