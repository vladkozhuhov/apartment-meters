using Application.Interfaces.Commands;
using Application.Models.MeterReading;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Реализация командного сервиса для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingCommand : IWaterMeterReadingCommand
{
    private readonly IWaterMeterReadingRepository _waterMeterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingCommand(IWaterMeterReadingRepository repository)
    {
        _waterMeterReadingRepository = repository;
    }

    /// <inheritdoc />
    public async Task<MeterReadingEntity> AddMeterReadingAsync(AddWaterMeterReadingDto dto)
    {
        dto.Validate();        
        
        var meterReading = new MeterReadingEntity
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            PrimaryColdWaterValue = dto.PrimaryColdWaterValue,
            PrimaryHotWaterValue = dto.PrimaryHotWaterValue,
            HasSecondaryMeter = dto.HasSecondaryMeter,
            SecondaryColdWaterValue = dto.SecondaryColdWaterValue,
            SecondaryHotWaterValue = dto.SecondaryHotWaterValue,
            ReadingDate = dto.ReadingDate,
            CreatedAt = DateTime.UtcNow
        };

        await _waterMeterReadingRepository.AddAsync(meterReading);
        return meterReading;
    }
    
    /// <inheritdoc />
    public async Task UpdateMeterReadingAsync(Guid id, UpdateWaterMeterReadingDto dto)
    {
        dto.Validate();
        
        var waterReading = await _waterMeterReadingRepository.GetByIdAsync(id);
        if (waterReading == null)
            throw new KeyNotFoundException($"MeterReading with user ID {dto.UserId} not found");

        waterReading.PrimaryColdWaterValue = dto.PrimaryColdWaterValue;
        waterReading.PrimaryHotWaterValue = dto.PrimaryHotWaterValue;
        waterReading.HasSecondaryMeter = dto.HasSecondaryMeter;
        waterReading.SecondaryColdWaterValue = dto.SecondaryColdWaterValue;
        waterReading.SecondaryHotWaterValue = dto.SecondaryHotWaterValue;
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