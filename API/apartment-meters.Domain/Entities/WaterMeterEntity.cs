using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Сущность для представления счетчика воды
/// </summary>
public class WaterMeterEntity
{
    [Key]
    public Guid Id { get; set; }

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
    /// Дата установки счетчика (только дата без времени)
    /// </summary>
    [Required]
    public DateOnly FactoryYear { get; set; }

    /// <summary>
    /// Навигационное свойство пользователя
    /// </summary>
    public UserEntity User { get; set; } = null!;

    /// <summary>
    /// Показания, связанные с этим счетчиком
    /// </summary>
    public ICollection<MeterReadingEntity> MeterReadings { get; set; } = new List<MeterReadingEntity>();
}