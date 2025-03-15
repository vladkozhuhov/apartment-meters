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
            FactoryYear = dto.FactoryYear,
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

        if (dto.PlaceOfWaterMeter.HasValue)
            waterMeter.PlaceOfWaterMeter = dto.PlaceOfWaterMeter.Value;
            
        if (dto.WaterType.HasValue)
            waterMeter.WaterType = dto.WaterType.Value;
            
        if (!string.IsNullOrEmpty(dto.FactoryNumber))
            waterMeter.FactoryNumber = dto.FactoryNumber;
            
        if (dto.FactoryYear.HasValue)
            waterMeter.FactoryYear = dto.FactoryYear.Value;

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