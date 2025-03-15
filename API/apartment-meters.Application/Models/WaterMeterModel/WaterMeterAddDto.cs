using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.WaterMeterModel;

/// <summary>
/// DTO для создания нового счетчика
/// </summary>
public class WaterMeterAddDto
{
    /// <summary>
    /// Идентификатор пользователя, которому принадлежит счетчик
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Место, где находится водомер
    /// </summary>
    [Required]
    public PlaceOfWaterMeter PlaceOfWaterMeter { get; set; }
    
    /// <summary>
    /// Тип воды, который передает счетчик
    /// </summary>
    [Required]
    public WaterType WaterType { get; set; }

    /// <summary>
    /// Заводской номер счетчика
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string FactoryNumber { get; set; } = null!;

    /// <summary>
    /// Дата установки счетчика (только дата в формате yyyy-MM-dd)
    /// </summary>
    [Required]
    public DateOnly FactoryYear { get; set; }
}