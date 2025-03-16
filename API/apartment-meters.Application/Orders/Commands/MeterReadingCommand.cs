using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Models.MeterReadingModel;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Application.Orders.Commands;

/// <summary>
/// Реализация командного сервиса для работы с показаниями водомеров
/// </summary>
public class MeterReadingCommand : IMeterReadingCommand
{
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly ILogger<MeterReadingCommand> _logger;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    /// <param name="logger">Сервис логирования</param>
    public MeterReadingCommand(
        IMeterReadingRepository repository,
        ILogger<MeterReadingCommand> logger)
    {
        _meterReadingRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<MeterReadingEntity> AddMeterReadingAsync(MeterReadingAddDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        _logger.LogInformation("Добавление нового показания для счетчика {WaterMeterId}", dto.WaterMeterId);
        
        var previousReading = await _meterReadingRepository.GetLastByWaterMeterIdAsync(dto.WaterMeterId);

        try
        {
            // Форматируем значение в нужный формат (00000,000)
            string formattedValue = FormatWaterValue(dto.WaterValue);
            
            // Преобразуем строковые значения в double для расчета разницы
            double currentValue = ParseWaterValue(formattedValue);
            double previousValue = previousReading != null ? ParseWaterValue(previousReading.WaterValue) : 0;
            
            // Проверяем, что новое показание не меньше предыдущего
            if (previousReading != null && currentValue < previousValue)
            {
                _logger.LogWarning("Попытка добавить показание ({CurrentValue}), которое меньше предыдущего ({PreviousValue})",
                    formattedValue, previousReading.WaterValue);
                throw new CustomException(ErrorType.MeterReadingLessThanPreviousError353);
            }
            
            // Вычисляем разницу и округляем до 3 десятичных знаков
            double differenceValue = previousReading != null ? Math.Round(currentValue - previousValue, 3) : 0;
            
            var meterReading = new MeterReadingEntity
            {
                Id = Guid.NewGuid(),
                WaterMeterId = dto.WaterMeterId,
                WaterValue = formattedValue,
                DifferenceValue = differenceValue,
                ReadingDate = dto.ReadingDate.ToUniversalTime(),
                CreatedAt = DateTime.UtcNow
            };

            await _meterReadingRepository.AddAsync(meterReading);
            _logger.LogInformation("Показание успешно добавлено с ID {MeterReadingId}", meterReading.Id);
            
            return meterReading;
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Неверный формат показания: {WaterValue}", dto.WaterValue);
            throw new CustomException(ErrorType.InvalidMeterReadingFormatError354);
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateMeterReadingAsync(Guid id, MeterReadingUpdateDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        
        _logger.LogInformation("Обновление показания с ID {MeterReadingId}", id);
        
        var waterReading = await _meterReadingRepository.GetByIdAsync(id);
        if (waterReading == null)
        {
            _logger.LogWarning("Показание с ID {MeterReadingId} не найдено", id);
            throw new CustomException(ErrorType.MeterReadingNotFoundError352);
        }

        var previousReading = await _meterReadingRepository.GetLastByWaterMeterIdAsync(dto.WaterMeterId);

        try
        {
            // Форматируем значение в нужный формат (00000,000)
            string formattedValue = FormatWaterValue(dto.WaterValue);
            
            // Преобразуем строковые значения в double для расчета разницы
            double currentValue = ParseWaterValue(formattedValue);
            double previousValue = previousReading != null ? ParseWaterValue(previousReading.WaterValue) : 0;

            // Проверяем, что новое показание не меньше предыдущего
            // Исключение: если мы редактируем само предыдущее показание
            if (previousReading != null && id != previousReading.Id && currentValue < previousValue)
            {
                _logger.LogWarning("Попытка обновить показание ({CurrentValue}), которое меньше предыдущего ({PreviousValue})",
                    formattedValue, previousReading.WaterValue);
                throw new CustomException(ErrorType.MeterReadingLessThanPreviousError353);
            }

            // Вычисляем разницу и округляем до 3 десятичных знаков
            double differenceValue = previousReading != null ? Math.Round(currentValue - previousValue, 3) : 0;

            waterReading.WaterMeterId = dto.WaterMeterId;
            waterReading.WaterValue = formattedValue;
            waterReading.DifferenceValue = differenceValue;

            await _meterReadingRepository.UpdateAsync(waterReading);
            _logger.LogInformation("Показание с ID {MeterReadingId} успешно обновлено", id);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Неверный формат показания: {WaterValue}", dto.WaterValue);
            throw new CustomException(ErrorType.InvalidMeterReadingFormatError354);
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteMeterReadingAsync(Guid id)
    {
        _logger.LogInformation("Удаление показания с ID {MeterReadingId}", id);
        
        var meterReading = await _meterReadingRepository.GetByIdAsync(id);
        if (meterReading == null)
        {
            _logger.LogWarning("Показание с ID {MeterReadingId} не найдено", id);
            throw new CustomException(ErrorType.MeterReadingNotFoundError352);
        }

        await _meterReadingRepository.DeleteAsync(meterReading);
        _logger.LogInformation("Показание с ID {MeterReadingId} успешно удалено", id);
    }
    
    /// <summary>
    /// Преобразует строковое значение показаний в double
    /// </summary>
    /// <param name="waterValue">Строковое значение в формате "00000,000"</param>
    /// <returns>Числовое значение показаний</returns>
    private double ParseWaterValue(string waterValue)
    {
        if (string.IsNullOrEmpty(waterValue))
        {
            throw new FormatException("Значение показания не может быть пустым");
        }
        
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
        {
            return "00000,000";
        }
            
        // Разделяем целую и дробную части
        string[] parts = input.Split(',');
        
        if (parts.Length != 2)
        {
            throw new FormatException($"Неправильный формат значения '{input}'. Должен быть в формате 'целое,дробное'.");
        }
            
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