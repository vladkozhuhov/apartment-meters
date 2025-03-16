using Application.Interfaces.Commands;
using Application.Models.WaterMeterModel;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands;

/// <summary>
/// Сервис для выполнения команд, связанных со счетчиками воды
/// </summary>
public class WaterMeterCommand : IWaterMeterCommand
{
    private readonly IWaterMeterRepository _waterMeterRepository;
    private readonly ILogger<WaterMeterCommand> _logger;

    /// <summary>
    /// Конструктор сервиса команд для счетчика воды
    /// </summary>
    /// <param name="waterMeterRepository">Репозиторий счетчиков воды</param>
    /// <param name="logger">Сервис логирования</param>
    public WaterMeterCommand(
        IWaterMeterRepository waterMeterRepository,
        ILogger<WaterMeterCommand> logger)
    {
        _waterMeterRepository = waterMeterRepository ?? throw new ArgumentNullException(nameof(waterMeterRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<WaterMeterEntity> AddWaterMeterAsync(WaterMeterAddDto dto)
    {
        _logger.LogInformation("Добавление нового счетчика воды для пользователя с ID {UserId}", dto.UserId);
        
        var waterMeter = new WaterMeterEntity
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            PlaceOfWaterMeter = dto.PlaceOfWaterMeter,
            WaterType = dto.WaterType,
            FactoryNumber = dto.FactoryNumber,
            FactoryYear = dto.FactoryYear,
        };

        await _waterMeterRepository.AddAsync(waterMeter);
        _logger.LogInformation("Счетчик воды с ID {WaterMeterId} успешно добавлен", waterMeter.Id);
        
        return waterMeter;
    }
    
    /// <inheritdoc />
    public async Task UpdateWaterMeterAsync(Guid id, WaterMeterUpdateDto dto)
    {
        _logger.LogInformation("Обновление счетчика воды с ID {WaterMeterId}", id);
        
        var waterMeter = await _waterMeterRepository.GetByIdAsync(id);
        if (waterMeter == null)
        {
            _logger.LogWarning("Счетчик воды с ID {WaterMeterId} не найден", id);
            throw new KeyNotFoundException($"Счетчик воды с ID {id} не найден");
        }

        if (dto.PlaceOfWaterMeter.HasValue)
            waterMeter.PlaceOfWaterMeter = dto.PlaceOfWaterMeter.Value;
            
        if (dto.WaterType.HasValue)
            waterMeter.WaterType = dto.WaterType.Value;
            
        if (!string.IsNullOrEmpty(dto.FactoryNumber))
            waterMeter.FactoryNumber = dto.FactoryNumber;
            
        if (dto.FactoryYear.HasValue)
            waterMeter.FactoryYear = dto.FactoryYear.Value;

        await _waterMeterRepository.UpdateAsync(waterMeter);
        _logger.LogInformation("Счетчик воды с ID {WaterMeterId} успешно обновлен", id);
    }

    /// <inheritdoc />
    public async Task DeleteWaterMeterAsync(Guid id)
    {
        _logger.LogInformation("Удаление счетчика воды с ID {WaterMeterId}", id);
        
        var waterMeter = await _waterMeterRepository.GetByIdAsync(id);
        if (waterMeter == null)
        {
            _logger.LogWarning("Счетчик воды с ID {WaterMeterId} не найден", id);
            throw new KeyNotFoundException($"Счетчик воды с ID {id} не найден");
        }

        await _waterMeterRepository.DeleteAsync(waterMeter);
        _logger.LogInformation("Счетчик воды с ID {WaterMeterId} успешно удален", id);
    }
}