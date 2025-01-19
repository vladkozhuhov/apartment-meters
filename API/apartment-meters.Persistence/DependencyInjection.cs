using Application.Services;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов Persistence
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <returns>Обновленная коллекция сервисов</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация DbContext с использованием PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("SecurityDb"),
                b => b.MigrationsAssembly("apartment-meters.Persistence") // сборка для миграций
            )
        );

        // Регистрация репозитория
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWaterMeterReadingRepository, WaterMeterReadingRepository>();
        
        // Регистрация сервиса для вычислений показаний водомеров
        services.AddScoped<MeterReadingCalculatorService>();
        
        return services;
    }
}