using Application.Interfaces.Commands;
using Application.Models.MeterReadingModel;
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
        var previousReading = await _waterMeterReadingRepository.GetLastByUserIdAsync(dto.UserId);
        var (primaryDifference, secondaryDifference) = CalculateDifferenceValues(
            int.Parse(dto.PrimaryColdWaterValue), 
            int.Parse(dto.PrimaryHotWaterValue), 
            dto.HasSecondaryMeter, 
            dto.SecondaryColdWaterValue, 
            dto.SecondaryHotWaterValue, 
            previousReading);
        
        var meterReading = new MeterReadingEntity
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            PrimaryColdWaterValue = dto.PrimaryColdWaterValue,
            PrimaryHotWaterValue = dto.PrimaryHotWaterValue,
            PrimaryTotalValue = int.Parse(dto.PrimaryColdWaterValue) + int.Parse(dto.PrimaryHotWaterValue),
            PrimaryDifferenceValue = primaryDifference,
            HasSecondaryMeter = dto.HasSecondaryMeter,
            SecondaryColdWaterValue = dto.SecondaryColdWaterValue,
            SecondaryHotWaterValue = dto.SecondaryHotWaterValue,
            SecondaryTotalValue = dto.HasSecondaryMeter 
                ? int.Parse(dto.SecondaryColdWaterValue ?? "0") + int.Parse(dto.SecondaryHotWaterValue ?? "0") 
                : null,
            SecondaryDifferenceValue = secondaryDifference,
            ReadingDate = dto.ReadingDate,
            CreatedAt = DateTime.UtcNow
        };

        await _waterMeterReadingRepository.AddAsync(meterReading);
        return meterReading;
    }
    
    /// <inheritdoc />
    public async Task UpdateMeterReadingAsync(Guid id, UpdateWaterMeterReadingDto dto)
    {
        var waterReading = await _waterMeterReadingRepository.GetByIdAsync(id);
        if (waterReading == null)
            throw new KeyNotFoundException($"MeterReading with user ID {dto.UserId} not found");

        var previousReading = await _waterMeterReadingRepository.GetLastByUserIdAsync(dto.UserId);
        var (primaryDifference, secondaryDifference) = CalculateDifferenceValues(
            int.Parse(dto.PrimaryColdWaterValue), 
            int.Parse(dto.PrimaryHotWaterValue), 
            dto.HasSecondaryMeter, 
            dto.SecondaryColdWaterValue, 
            dto.SecondaryHotWaterValue, 
            previousReading);
        
        waterReading.PrimaryColdWaterValue = dto.PrimaryColdWaterValue;
        waterReading.PrimaryHotWaterValue = dto.PrimaryHotWaterValue;
        waterReading.PrimaryTotalValue = int.Parse(dto.PrimaryColdWaterValue) + int.Parse(dto.PrimaryHotWaterValue);
        waterReading.PrimaryDifferenceValue = primaryDifference;
        waterReading.HasSecondaryMeter = dto.HasSecondaryMeter;
        waterReading.SecondaryColdWaterValue = dto.SecondaryColdWaterValue;
        waterReading.SecondaryHotWaterValue = dto.SecondaryHotWaterValue;
        waterReading.SecondaryTotalValue = dto.HasSecondaryMeter 
            ? int.Parse(dto.SecondaryColdWaterValue ?? "0") + int.Parse(dto.SecondaryHotWaterValue ?? "0") 
            : null;
        waterReading.SecondaryDifferenceValue = secondaryDifference;
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
    
    private (int PrimaryDifference, int? SecondaryDifference) CalculateDifferenceValues(
        int primaryColdWaterValue, 
        int primaryHotWaterValue, 
        bool hasSecondaryMeter, 
        string? secondaryColdWaterValue, 
        string? secondaryHotWaterValue, 
        MeterReadingEntity? previousReading)
    {
        var primaryTotalValue = primaryColdWaterValue + primaryHotWaterValue;
        var secondaryTotalValue = hasSecondaryMeter
            ? int.Parse(secondaryColdWaterValue ?? "0") + int.Parse(secondaryHotWaterValue ?? "0") 
            : (int?)null;

        var primaryDifferenceValue = previousReading != null 
            ? primaryTotalValue - previousReading.PrimaryTotalValue 
            : 0;

        var secondaryDifferenceValue = previousReading?.SecondaryTotalValue != null && hasSecondaryMeter
            ? secondaryTotalValue - previousReading.SecondaryTotalValue 
            : null;

        return (primaryDifferenceValue, secondaryDifferenceValue);
    }
}