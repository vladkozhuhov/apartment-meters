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
    /// Дата снятия показания
    /// </summary>
    public DateTime ReadingDate { get; set; }
}