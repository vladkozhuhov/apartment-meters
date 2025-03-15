using Application.Interfaces.Commands;
using Application.Models.MeterReadingModel;
using Domain.Entities;
using Domain.Repositories;
using System.Globalization;

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

        // Форматируем значение в нужный формат (00000,000)
        string formattedValue = FormatWaterValue(dto.WaterValue);
        
        // Преобразуем строковые значения в double для расчета разницы
        double currentValue = ParseWaterValue(formattedValue);
        double previousValue = previousReading != null ? ParseWaterValue(previousReading.WaterValue) : 0;
        
        // Проверяем, что новое показание не меньше предыдущего
        if (previousReading != null && currentValue < previousValue)
        {
            throw new InvalidOperationException($"Новое показание ({formattedValue}) не может быть меньше предыдущего ({previousReading.WaterValue}).");
        }
        
        var meterReading = new MeterReadingEntity
        {
            Id = Guid.NewGuid(),
            WaterMeterId = dto.WaterMeterId,
            WaterValue = formattedValue,
            DifferenceValue = previousReading != null ? currentValue - previousValue : 0,
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

        // Форматируем значение в нужный формат (00000,000)
        string formattedValue = FormatWaterValue(dto.WaterValue);
        
        // Преобразуем строковые значения в double для расчета разницы
        double currentValue = ParseWaterValue(formattedValue);
        double previousValue = previousReading != null ? ParseWaterValue(previousReading.WaterValue) : 0;

        // Проверяем, что новое показание не меньше предыдущего
        // Исключение: если мы редактируем само предыдущее показание
        if (previousReading != null && id != previousReading.Id && currentValue < previousValue)
        {
            throw new InvalidOperationException($"Новое показание ({formattedValue}) не может быть меньше предыдущего ({previousReading.WaterValue}).");
        }

        waterReading.WaterMeterId = dto.WaterMeterId;
        waterReading.WaterValue = formattedValue;
        waterReading.DifferenceValue = previousReading != null ? currentValue - previousValue : 0;

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
    
    /// <summary>
    /// Преобразует строковое значение показаний в double
    /// </summary>
    /// <param name="waterValue">Строковое значение в формате "00000,000"</param>
    /// <returns>Числовое значение показаний</returns>
    private double ParseWaterValue(string waterValue)
    {
        // Заменяем запятую на точку для корректного парсинга в double
        string normalizedValue = waterValue.Replace(',', '.');
        if (double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }
        
        throw new FormatException($"Невозможно преобразовать значение '{waterValue}' в число.");
    }
    
    /// <summary>
    /// Форматирует значение показаний в формат "00000,000"
    /// </summary>
    /// <param name="input">Исходное значение (например, "35,168")</param>
    /// <returns>Отформатированное значение (например, "00035,168")</returns>
    private string FormatWaterValue(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "00000,000";
            
        // Разделяем целую и дробную части
        string[] parts = input.Split(',');
        
        if (parts.Length != 2)
            throw new FormatException($"Неправильный формат значения '{input}'. Должен быть в формате 'целое,дробное'.");
            
        string integerPart = parts[0];
        string fractionalPart = parts[1];
        
        // Дополняем целую часть нулями слева до 5 цифр
        string formattedInteger = integerPart.PadLeft(5, '0');
        
        // Если дробная часть короче 3 цифр, дополняем нулями справа
        string formattedFractional = fractionalPart.Length < 3
            ? fractionalPart.PadRight(3, '0')
            : fractionalPart.Substring(0, 3); // Обрезаем, если длиннее 3 цифр
            
        return $"{formattedInteger},{formattedFractional}";
    }
}