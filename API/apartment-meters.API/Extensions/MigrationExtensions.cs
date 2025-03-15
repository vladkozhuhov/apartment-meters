using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace API.Extensions
{
    /// <summary>
    /// Расширения для работы с миграциями
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Применяет миграции к базе данных при запуске приложения
        /// </summary>
        /// <param name="app">Web-приложение</param>
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Console.WriteLine("Применяем миграции к базе данных...");
                
                // Попытка применить миграции несколько раз с задержкой
                // (для того чтобы дождаться запуска PostgreSQL в Docker)
                const int maxRetries = 10;
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        dbContext.Database.Migrate();
                        Console.WriteLine("Миграции успешно применены!");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при применении миграций (попытка {retry+1}/{maxRetries}): {ex.Message}");
                        if (retry == maxRetries - 1)
                            throw;
                            
                        // Ждем перед следующей попыткой
                        Thread.Sleep(2000);
                    }
                }
            }
        }
    }
} 