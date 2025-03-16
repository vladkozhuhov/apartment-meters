using Application.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("SecurityDb"),
                b => b.MigrationsAssembly("apartment-meters.Persistence") // сборка для миграций
            )
        );

        // Регистрируем фоновые задачи как синглтон, чтобы сохранять состояние всех задач
        services.AddSingleton<IBackgroundTaskService, BackgroundTaskService>();
        
        return services;
    }
}