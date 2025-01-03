using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Application.Services;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Contexts;
using Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

#region Настройка подключения к PostgreSQL и другим сервисам

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDb")));

#endregion

#region Register repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWaterMeterReadingRepository, WaterMeterReadingRepository>();

#endregion

#region Register services

builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

builder.Services.AddScoped<IWaterMeterReadingCommandService, WaterMeterReadingCommandService>();
builder.Services.AddScoped<IWaterMeterReadingQueryService, WaterMeterReadingQueryService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();