using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Application.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Расширения для настройки DI сервисов уровня приложения
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Добавление сервисов уровня приложения в DI-контейнер
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Дополненная коллекция сервисов</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация команд
        services.AddScoped<IMeterReadingCommand, MeterReadingCommand>();
        services.AddScoped<IUserCommand, UserCommand>();
        services.AddScoped<IWaterMeterCommand, WaterMeterCommand>();
        
        // Регистрация запросов
        services.AddScoped<IMeterReadingQuery, MeterReadingQuery>();
        services.AddScoped<IUserQuery, UserQuery>();
        services.AddScoped<IWaterMeterQuery, WaterMeterQuery>();
        
        // Аутентификация
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        return services;
    }
} 