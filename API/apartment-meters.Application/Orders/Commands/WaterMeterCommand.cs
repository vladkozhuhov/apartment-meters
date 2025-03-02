using Application.Interfaces.Commands;
using Application.Models.WaterMeterModel;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Сервис для выполнения команд, связанных со счетчиками
/// </summary>
public class WaterMeterCommand : IWaterMeterCommand
{
    private readonly IWaterMeterRepository _waterMaterRepository;

    /// <summary>
    /// Конструктор сервиса команд для счетчика
    /// </summary>
    /// <param name="waterMaterRepository">Репозиторий счетчика</param>
    public WaterMeterCommand(IWaterMeterRepository waterMaterRepository)
    {
        _waterMaterRepository = waterMaterRepository;
    }

    /// <inheritdoc />
    public async Task<WaterMeterEntity> AddWaterMeterAsync(WaterMeterAddDto dto)
    {
        var waterMeter = new WaterMeterEntity
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            PlaceOfWaterMeter = dto.PlaceOfWaterMeter,
            WaterType = dto.WaterType,
            FactoryNumber = dto.FactoryNumber,
            FactoryYear = dto.FactoryYear.ToUniversalTime(),
        };

        await _waterMaterRepository.AddAsync(waterMeter);
        return waterMeter;
    }
    
    /// <inheritdoc />
    public async Task UpdateWaterMeterAsync(Guid id, WaterMeterUpdateDto dto)
    {
        var waterMeter = await _waterMaterRepository.GetByIdAsync(id);
        if (waterMeter == null)
            throw new KeyNotFoundException($"WaterMeter with User ID {id} not found");

        waterMeter.PlaceOfWaterMeter = dto.PlaceOfWaterMeter ?? waterMeter.PlaceOfWaterMeter;
        waterMeter.WaterType = dto.WaterType ?? waterMeter.WaterType;
        waterMeter.FactoryNumber = dto.FactoryNumber ?? waterMeter.FactoryNumber;
        waterMeter.FactoryYear = dto.FactoryYear ?? waterMeter.FactoryYear;

        await _waterMaterRepository.UpdateAsync(waterMeter);
    }

    /// <inheritdoc />
    public async Task DeleteWaterMeterAsync(Guid id)
    {
        var waterMeter = await _waterMaterRepository.GetByIdAsync(id);
        if (waterMeter == null)
            throw new KeyNotFoundException($"WaterMeter with ID {id} not found.");

        await _waterMaterRepository.DeleteAsync(waterMeter);
    }
}