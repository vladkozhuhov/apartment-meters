namespace Application.Models.MeterReading;

/// <summary>
/// DTO для обновления показания водомера
/// </summary>
public class UpdateWaterMeterReadingDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Показание холодной воды для первого счетчика
    /// </summary>
    public string PrimaryColdWaterValue { get; set; } = "00000";

    /// <summary>
    /// Показание горячей воды для первого счетчика
    /// </summary>
    public string PrimaryHotWaterValue { get; set; } = "00000";
    
    /// <summary>
    /// Сумма показаний холодной и горячей воды для основного счетчика
    /// </summary>
    public int PrimaryTotalValue { get; set; }

    /// <summary>
    /// Разница сумм показаний между текущей и предыдущей записью для основного счетчика
    /// </summary>
    public int PrimaryDifferenceValue { get; set; }

    /// <summary>
    /// Флаг, указывающий, есть ли у пользователя второй счетчик
    /// </summary>
    public bool HasSecondaryMeter { get; set; }

    /// <summary>
    /// Показание холодной воды для второго счетчика
    /// </summary>
    public string? SecondaryColdWaterValue { get; set; } = "00000";

    /// <summary>
    /// Показание горячей воды для второго счетчика
    /// </summary>
    public string? SecondaryHotWaterValue { get; set; } = "00000";
    
    /// <summary>
    /// Сумма показаний холодной и горячей воды для дополнительного счетчика
    /// </summary>
    public int? SecondaryTotalValue { get; set; }

    /// <summary>
    /// Разница сумм показаний между текущей и предыдущей записью для дополнительного счетчика
    /// </summary>
    public int? SecondaryDifferenceValue { get; set; }

    /// <summary>
    /// Дата снятия показания
    /// </summary>
    public DateTime ReadingDate { get; set; }
    
    /// <summary>
    /// Метод для валидации данных
    /// </summary>
    public void Validate()
    {
        // Валидация значений для первого счетчика
        if (!int.TryParse(PrimaryColdWaterValue, out var coldWaterValue))
            throw new ArgumentException("Invalid value for PrimaryColdWaterValue.");

        if (!int.TryParse(PrimaryHotWaterValue, out var hotWaterValue))
            throw new ArgumentException("Invalid value for PrimaryHotWaterValue.");

        PrimaryTotalValue = coldWaterValue + hotWaterValue;

        // Валидация значений для второго счетчика
        if (HasSecondaryMeter)
        {
            if (!int.TryParse(SecondaryColdWaterValue, out var secondaryColdWaterValue))
                throw new ArgumentException("Invalid value for SecondaryColdWaterValue.");

            if (!int.TryParse(SecondaryHotWaterValue, out var secondaryHotWaterValue))
                throw new ArgumentException("Invalid value for SecondaryHotWaterValue.");

            SecondaryTotalValue = secondaryColdWaterValue + secondaryHotWaterValue;
        }
    }
}