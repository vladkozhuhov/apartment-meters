using System.Reflection;
using System.Threading.RateLimiting;
using API.Converters;
using API.Extensions;
using Application;
using Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Persistence;

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
    // Настройка контроллеров и JSON сериализации
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Добавляем конвертер для типа DateOnly
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        })
        .AddFluentValidation(); // Добавляем поддержку FluentValidation
        
    // Регистрация валидаторов
    services.AddValidatorsFromAssemblyContaining<MeterReadingAddDtoValidator>();
    
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
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100, // Максимальное количество запросов
                    Window = TimeSpan.FromMinutes(1) // За 1 минуту
                }));
                
        // Добавляем особые лимиты для некоторых конечных точек
        options.AddPolicy("api-readings", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20, // Более строгий лимит для API показаний
                    Window = TimeSpan.FromMinutes(1)
                }));
                
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
    });

    // Настройка подключения к PostgreSQL и другим сервисам
    services.AddPersistenceServices(configuration);
    services.AddInfrastructureServices(configuration);
    services.AddApplicationServices(configuration);

    // Регистрация сервиса аутентификации перенесена в слой приложения (DependencyInjection.cs)
}

void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
{
    // Автоматически применяем миграции при запуске
    app.ApplyMigrations();

    // Глобальный обработчик исключений
    app.UseGlobalExceptionHandler();

    // Ограничение скорости запросов
    app.UseRateLimiter();

    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apartment Meters API v1");
        c.RoutePrefix = "swagger"; // Swagger UI будет доступен по пути /swagger
    });

    // CORS
    app.UseCors(builder =>
    {
        builder.WithHeaders().AllowAnyHeader();
        builder.WithOrigins("http://localhost:3000") // Добавляем поддержку swaggerUI
               .SetIsOriginAllowed(origin => true); // Разрешаем все источники во время разработки
        builder.WithMethods().AllowAnyMethod();
        builder.AllowCredentials(); // Разрешаем передачу учетных данных
    });

    // Аутентификация и авторизация
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Временно отключаем Antiforgery для решения проблемы с авторизацией в Swagger
    // app.UseAntiforgery();
    
    app.MapControllers();
}