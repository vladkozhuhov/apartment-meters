namespace Application.Models;

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
    /// Значение показания
    /// </summary>
    public decimal ColdWaterValue { get; set; }
    
    /// <summary>
    /// Значение показания
    /// </summary>
    public decimal HotWaterValue { get; set; }

    /// <summary>
    /// Дата снятия показания
    /// </summary>
    public DateTime ReadingDate { get; set; }
}