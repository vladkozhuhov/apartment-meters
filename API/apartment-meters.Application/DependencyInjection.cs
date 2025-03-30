using System.Reflection;
using Application.Behaviors;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Interfaces.Services;
using Application.Models.MeterReadingModel;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Application.Services;
using Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Класс для регистрации сервисов уровня приложения в DI-контейнере
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Добавляет сервисы уровня приложения в DI-контейнер
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <returns>Коллекция сервисов с добавленными сервисами уровня приложения</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация MediatR
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        // Регистрация валидаторов
        services.AddFluentValidationAutoValidation();
        
        // Регистрация всех валидаторов, кроме MeterReadingAddDtoValidator
        // (он регистрируется отдельно ниже, т.к. требует внедрения зависимостей)
        services.AddValidatorsFromAssemblyContaining<MeterReadingUpdateDtoValidator>();
        
        // Регистрация валидатора MeterReadingAddDtoValidator с поддержкой внедрения зависимостей
        services.AddScoped<IValidator<MeterReadingAddDto>, MeterReadingAddDtoValidator>(
            provider => new MeterReadingAddDtoValidator(provider.GetRequiredService<IErrorHandlingService>()));
        
        // Регистрация сервисов команд
        services.AddScoped<IMeterReadingCommand, MeterReadingCommand>();
        services.AddScoped<IUserCommand, UserCommand>();
        services.AddScoped<IWaterMeterCommand, WaterMeterCommand>();
        
        // Регистрация сервисов запросов
        services.AddScoped<IMeterReadingQuery, MeterReadingQuery>();
        services.AddScoped<IUserQuery, UserQuery>();
        services.AddScoped<IWaterMeterQuery, WaterMeterQuery>();
        
        // Регистрация сервиса обработки ошибок
        services.AddScoped<IErrorHandlingService, ErrorHandlingService>();
        
        return services;
    }
} 