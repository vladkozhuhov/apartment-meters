using System.Reflection;
using System.Threading.RateLimiting;
using API.Converters;
using API.Extensions;
using API.Filters;
using API.Middleware;
using Application;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

// Конфигурация HTTP pipeline
ConfigureMiddleware(app, app.Environment);

app.Run();

// Методы конфигурации для улучшения читаемости
void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
    
    // Настройка контроллеров и JSON сериализации
    services.AddControllers(options =>
        {
            // Регистрируем глобальный фильтр исключений
            options.Filters.Add<ApiExceptionFilterAttribute>();
        })
        .AddJsonOptions(options =>
        {
            // Добавляем конвертер для типа DateOnly
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        });
        
    // Добавляем поддержку FluentValidation (новым способом)
    services.AddFluentValidationAutoValidation();
    services.AddFluentValidationClientsideAdapters();
    
    // Регистрация валидаторов перенесена в Application/DependencyInjection.cs
    
    // Добавляем HttpContextAccessor для доступа к HttpContext
    services.AddHttpContextAccessor();
    
    // Регистрируем наш AuthorizationHandler
    services.AddSingleton<IAuthorizationHandler, ApiAuthMeAuthorizationHandler>();
    
    // Добавляем Basic Authentication для Swagger UI
    services.AddAuthentication()
        .AddScheme<AuthenticationSchemeOptions, SwaggerBasicAuthenticationHandler>("SwaggerBasicAuth", null);
    
    // Добавляем защиту от CSRF-атак
    services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.Name = "XSRF-TOKEN";
        options.Cookie.HttpOnly = false; // Чтобы JavaScript мог читать токен
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    
    // Добавляем ограничение скорости запросов (Rate Limiting)
    services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            // Проверяем админа по настоящим путям в системе
            bool isAdmin = context.Request.Path.StartsWithSegments("/admin") ||
                           // Используем существующую систему ролей/аутентификации
                           context.User.Identity?.IsAuthenticated == true;
            
            if (isAdmin)
            {
                // Значительно увеличиваем лимит для админских путей
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "admin",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 5000, // Очень высокий лимит для админов
                        Window = TimeSpan.FromMinutes(1)
                    });
            }
            
            // Для обычных пользователей
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100, // Максимальное количество запросов
                    Window = TimeSpan.FromMinutes(1) // За 1 минуту
                });
        });
                
        // Добавляем особые лимиты для некоторых конечных точек
        options.AddPolicy("api-readings", context =>
        {
            // Проверка админа по тем же правилам
            bool isAdmin = context.Request.Path.StartsWithSegments("/admin") ||
                           context.User.Identity?.IsAuthenticated == true;
            
            if (isAdmin)
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "admin",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 3000, // Увеличиваем лимит для админских запросов
                        Window = TimeSpan.FromMinutes(1)
                    });
            }
            
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20, // Более строгий лимит для API показаний
                    Window = TimeSpan.FromMinutes(1)
                });
        });
                
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";
            
            var response = new
            {
                Code = 429,
                Message = "Слишком много запросов. Пожалуйста, повторите попытку позже."
            };
            
            await context.HttpContext.Response.WriteAsJsonAsync(response, token);
        };
    });
        
    // Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        // Указываем для Swagger, что тип DateOnly должен обрабатываться как строка с форматом date
        options.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date"
        });
        
        // Добавляем информацию о версии API
        options.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Apartment Meters API", 
            Version = "v1",
            Description = "API для управления показаниями счетчиков в квартирах"
        });
        
        // Включаем комментарии XML для Swagger
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Добавляем поддержку JWT
       options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
       {
           In = ParameterLocation.Header,
           Description = "Введите токен в формате Bearer {токен}",
           Name = "Authorization",
           Type = SecuritySchemeType.ApiKey
       });

       options.AddSecurityRequirement(new OpenApiSecurityRequirement
       {
           {
               new OpenApiSecurityScheme
               {
                   Reference = new OpenApiReference
                   {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Bearer"
                   }
               },
               new string[] {}
           }
       });
       
       // Добавляем Basic Auth для Swagger
       options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
       {
           Type = SecuritySchemeType.Http,
           Scheme = "basic",
           Description = "Базовая аутентификация для Swagger"
       });
       
       options.AddSecurityRequirement(new OpenApiSecurityRequirement
       {
           {
               new OpenApiSecurityScheme
               {
                   Reference = new OpenApiReference
                   {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Basic"
                   }
               },
               new string[] {}
           }
       });
    });

    // Настройка подключения к PostgreSQL и другим сервисам
    services.AddPersistenceServices(configuration);
    services.AddInfrastructureServices(configuration);
    services.AddApplicationServices(configuration);

    // Добавляем фоновую службу для отправки уведомлений
    services.AddHostedService<MeterReadingNotificationService>();
}

void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
{
    // Автоматически применяем миграции при запуске
    app.ApplyMigrations();
    
    // Запускаем миграцию паролей
    app.MigratePasswords();

    // CORS должен быть размещен перед другими middleware для правильной работы
    app.UseCors();

    app.UseRateLimiter();

    // Swagger UI с защитой
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apartment Meters API v1");
        c.RoutePrefix = "swagger"; // Swagger UI будет доступен по пути /swagger
        c.EnablePersistAuthorization(); // Важно для сохранения токена аутентификации
        c.DisplayRequestDuration(); // Показывает длительность запросов
        
        // Дополнительно можно настроить JavaScript для улучшения работы с аутентификацией
        c.DefaultModelsExpandDepth(-1); // Скрываем модели по умолчанию
        c.DocExpansion(DocExpansion.None); // Не раскрываем операции по умолчанию
    });

    // Добавляем аутентификацию для Swagger
    app.UseMiddleware<SwaggerBasicAuthMiddleware>();

    // Добавляем наш кастомный middleware перед стандартным
    app.UseCustomAuthorization();

    // Аутентификация и авторизация
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();
}