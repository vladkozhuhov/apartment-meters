using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Application.Validators;
using Domain.Repositories;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Contexts;
using Persistence.Repositories;
using API.Converters;
using API.Extensions;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Добавляем конвертер для типа DateOnly
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Указываем для Swagger, что тип DateOnly должен обрабатываться как строка с форматом date
    options.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});

// builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//     .AddNegotiate();

// builder.Services.AddAuthorization(options =>
// {
//     options.FallbackPolicy = options.DefaultPolicy;
// });

#region Настройка подключения к PostgreSQL и другим сервисам

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDb")));

#endregion

#region Register repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWaterMeterRepository, WaterMeterRepository>();
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();

#endregion

#region Register services

builder.Services.AddScoped<IUserCommand, UserCommand>();
builder.Services.AddScoped<IUserQuery, UserQuery>();

builder.Services.AddScoped<IWaterMeterCommand, WaterMeterCommand>();
builder.Services.AddScoped<IWaterMeterQuery, WaterMeterQuery>();

builder.Services.AddScoped<IMeterReadingCommand, MeterReadingCommand>();
builder.Services.AddScoped<IMeterReadingQuery, MeterReadingQuery>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

#endregion

var app = builder.Build();

// Автоматически применяем миграции при запуске
app.ApplyMigrations();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors(builder =>
{
    builder.WithHeaders().AllowAnyHeader();
    builder.WithOrigins("http://localhost:3000");
    builder.WithMethods().AllowAnyMethod();
});

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();