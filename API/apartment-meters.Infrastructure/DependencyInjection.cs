using Application.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Auth;
using System.Security.Claims;
using Application.Models.NotificationModel;

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
        
        // Конфигурация настроек Web Push уведомлений
        services.Configure<PushNotificationSettings>(
            configuration.GetSection("PushNotificationSettings"));
            
        // Регистрация сервиса Web Push уведомлений
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        
        // Configure JWT
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);

        var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                // Не автоматически выбираем провайдер для проверки ключа подписи
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
            
            // Добавляем обработку событий для диагностики
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Ошибка аутентификации: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var roles = context.Principal.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();
                        
                    Console.WriteLine($"Токен валидирован. ID пользователя: {context.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}, роли: {string.Join(", ", roles)}");
                    return Task.CompletedTask;
                }
            };
        });
        
        // Настраиваем авторизацию для поддержки ролей
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
            
            // Добавляем политику для API/auth/me, которая требует только аутентификации
            options.AddPolicy("AuthenticatedUserAPI", policy => 
                policy.AddRequirements(new ApiAuthMeRequirement()));
        });

        // Register services
        services.AddTransient<IJwtService, JwtService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}