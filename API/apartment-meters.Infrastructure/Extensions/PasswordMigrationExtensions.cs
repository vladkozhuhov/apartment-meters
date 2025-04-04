using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

/// <summary>
/// Расширения для миграции паролей
/// </summary>
public static class PasswordMigrationExtensions
{
    /// <summary>
    /// Запускает миграцию паролей при старте приложения
    /// </summary>
    /// <param name="app">Экземпляр WebApplication</param>
    /// <returns>Тот же экземпляр WebApplication для fluent API</returns>
    public static WebApplication MigratePasswords(this WebApplication app)
    {
        // Создаем новый скоуп для миграции паролей
        using var scope = app.Services.CreateScope();
        var passwordMigrationService = scope.ServiceProvider.GetRequiredService<PasswordMigrationService>();
        
        // Запускаем миграцию паролей асинхронно
        Task.Run(() => passwordMigrationService.MigratePasswords()).Wait();
        
        return app;
    }
} 