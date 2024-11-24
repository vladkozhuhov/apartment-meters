using Application.Interfaces.Commands;
using Application.Models;
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
    public async Task AddMeterReadingAsync(AddWaterMeterReadingDto dto)
    {
        var meterReading = new MeterReading
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ColdWaterValue = dto.ColdWaterValue,
            HotWaterValue = dto.HotWaterValue,
            ReadingDate = dto.ReadingDate,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(meterReading);
    }
    
    /// <inheritdoc />
    public async Task DeleteMeterReadingAsync(Guid id)
    {
        var meterReading = await _repository.GetByIdAsync(id);
        if (meterReading == null)
        {
            throw new KeyNotFoundException($"Показание с ID {id} не найдено.");
        }

        await _repository.DeleteAsync(meterReading);
    }
}