using Application.Interfaces.Commands;
using Application.Models.MeterReadingModel;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Реализация командного сервиса для работы с показаниями водомеров
/// </summary>
public class MeterReadingCommand : IMeterReadingCommand
{
    private readonly IMeterReadingRepository _meterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public MeterReadingCommand(IMeterReadingRepository repository)
    {
        _meterReadingRepository = repository;
    }

    /// <inheritdoc />
    public async Task<MeterReadingEntity> AddMeterReadingAsync(MeterReadingAddDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var previousReading = await _meterReadingRepository.GetLastByWaterMeterIdAsync(dto.WaterMeterId);
        int previousTotalValue = previousReading?.TotalValue ?? 0;
        int differenceValue = previousReading != null 
            ? CalculateDifferenceValues(int.Parse(dto.WaterValue), previousReading) 
            : int.Parse(dto.WaterValue);

        var meterReading = new MeterReadingEntity
        {
            Id = Guid.NewGuid(),
            WaterMeterId = dto.WaterMeterId,
            WaterValue = dto.WaterValue,
            TotalValue = previousTotalValue + int.Parse(dto.WaterValue),
            DifferenceValue = differenceValue,
            ReadingDate = dto.ReadingDate.ToUniversalTime(),
            CreatedAt = DateTime.UtcNow
        };

        await _meterReadingRepository.AddAsync(meterReading);
        return meterReading;
    }
    
    /// <inheritdoc />
    public async Task UpdateMeterReadingAsync(Guid id, MeterReadingUpdateDto dto)
    {
        var waterReading = await _meterReadingRepository.GetByIdAsync(id);
        if (waterReading == null)
            throw new KeyNotFoundException($"MeterReading with ID {dto.WaterMeterId} not found");

        var previousReading = await _meterReadingRepository.GetLastByWaterMeterIdAsync(dto.WaterMeterId);
        int previousTotalValue = previousReading?.TotalValue ?? 0;
        int differenceValue = previousReading != null 
            ? CalculateDifferenceValues(int.Parse(dto.WaterValue), previousReading) 
            : int.Parse(dto.WaterValue);

        waterReading.WaterMeterId = dto.WaterMeterId;
        waterReading.WaterValue = dto.WaterValue;
        waterReading.TotalValue = previousTotalValue + int.Parse(dto.WaterValue);
        waterReading.DifferenceValue = differenceValue;

        await _meterReadingRepository.UpdateAsync(waterReading);
    }
    
    /// <inheritdoc />
    public async Task DeleteMeterReadingAsync(Guid id)
    {
        var meterReading = await _meterReadingRepository.GetByIdAsync(id);
        if (meterReading == null)
        {
            throw new KeyNotFoundException($"Показание с ID {id} не найдено.");
        }

        await _meterReadingRepository.DeleteAsync(meterReading);
    }
    
    private int CalculateDifferenceValues(int waterValue, MeterReadingEntity? previousReading)
    {
        var differenceValue = previousReading != null 
            ? waterValue - previousReading.TotalValue 
            : 0;

        return differenceValue;
    }
}