namespace Application.Models;

/// <summary>
/// DTO для добавления показания водомера
/// </summary>
public class AddWaterMeterReadingDto
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