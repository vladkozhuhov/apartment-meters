using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence.Contexts;
using Persistence.Repositories;
using Persistence.Repositories.Cached;
using Application.Interfaces.Repositories;
using Persistence.Repositories.Base;
using Scrutor;

namespace Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Добавление сервисов персистентности в DI-контейнер
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Дополненная коллекция сервисов</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SecurityDb")));

        // Регистрация базовых репозиториев
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWaterMeterRepository, WaterMeterRepository>();
        services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        
        // Регистрация кэширующих репозиториев для запросов (ICachedRepository)
        services.AddMemoryCache();
        
        // Добавляем распределенный кэш в памяти для IDistributedCache
        services.AddDistributedMemoryCache();
        
        services.AddScoped<ICachedRepository<Domain.Entities.MeterReadingEntity, Guid>, CachedMeterReadingRepository>();
        services.AddScoped<ICachedRepository<Domain.Entities.UserEntity, Guid>, CachedUserRepository>();
        services.AddScoped<ICachedRepository<Domain.Entities.WaterMeterEntity, Guid>, CachedWaterMeterRepository>();
        
        // Дополнительная регистрация расширенного кэшированного репозитория для счетчиков воды
        services.AddScoped<CachedWaterMeterRepository>();
        
        // Декорирование базовых репозиториев для прозрачного кэширования
        services.Decorate<IUserRepository, CachedUserRepositoryDecorator>();
        services.Decorate<IWaterMeterRepository, CachedWaterMeterRepositoryDecorator>();
        services.Decorate<IMeterReadingRepository, CachedMeterReadingRepositoryDecorator>();
        
        return services;
    }
}