using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.WaterMeterModel;

/// <summary>
/// DTO для обновления данных счетчика
/// </summary>
public class WaterMeterUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    public Guid? UserId { get; set; }

    /// <summary>
    /// Место, где находится водомер
    /// </summary>
    public PlaceOfWaterMeter? PlaceOfWaterMeter { get; set; }
    
    /// <summary>
    /// Тип воды, который передает счетчик
    /// </summary>
    public WaterType? WaterType { get; set; }

    /// <summary>
    /// Заводской номер счетчика
    /// </summary>
    [MaxLength(10)]
    public string? FactoryNumber { get; set; }

    /// <summary>
    /// Дата установки счетчика (только дата в формате yyyy-MM-dd)
    /// </summary>
    public DateOnly? FactoryYear { get; set; }
}