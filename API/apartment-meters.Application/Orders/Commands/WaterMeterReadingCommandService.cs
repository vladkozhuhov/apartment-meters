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
    private readonly IWaterMeterReadingRepository _waterMeterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingCommandService(IWaterMeterReadingRepository repository)
    {
        _waterMeterReadingRepository = repository;
    }

    /// <inheritdoc />
    public async Task<MeterReading> AddMeterReadingAsync(AddWaterMeterReadingDto dto)
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

        await _waterMeterReadingRepository.AddAsync(meterReading);
        return meterReading;
    }
    
    /// <inheritdoc />
    public async Task UpdateMeterReadingAsync(UpdateWaterMeterReadingDto dto)
    {
        var waterReading = await _waterMeterReadingRepository.GetByIdAsync(dto.UserId);
        if (waterReading == null)
            throw new KeyNotFoundException($"MeterReading with user ID {dto.UserId} not found");

        waterReading.ColdWaterValue = dto.ColdWaterValue;
        waterReading.HotWaterValue = dto.HotWaterValue;
        waterReading.ReadingDate = dto.ReadingDate;

        await _waterMeterReadingRepository.UpdateAsync(waterReading);
    }
    
    /// <inheritdoc />
    public async Task DeleteMeterReadingAsync(Guid id)
    {
        var meterReading = await _waterMeterReadingRepository.GetByIdAsync(id);
        if (meterReading == null)
        {
            throw new KeyNotFoundException($"Показание с ID {id} не найдено.");
        }

        await _waterMeterReadingRepository.DeleteAsync(meterReading);
    }
}