using Application.Interfaces.Commands;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Реализация командного сервиса для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingCommandService : IWaterMeterReadingCommandService
{
    private readonly IWaterMeterReadingRepository _repository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingCommandService(IWaterMeterReadingRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task AddMeterReadingAsync(MeterReading meterReading)
    {
        await _repository.AddAsync(meterReading);
    }
}