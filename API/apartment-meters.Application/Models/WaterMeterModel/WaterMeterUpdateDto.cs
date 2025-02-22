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
    
    /// <summary>
    /// Место, где находится водомер
    /// </summary>
    [Required]
    public PlaceOfWaterMeter? PlaceOfWaterMeter { get; set; }
    
    /// <summary>
    /// Тип воды, который передает счетчик
    /// </summary>
    [Required]
    public WaterType? WaterType { get; set; }

    /// <summary>
    /// Заводской номер счетчика
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string? FactoryNumber { get; set; } = null!;

    /// <summary>
    /// Год выпуска счетчика
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FactoryYear { get; set; }
}