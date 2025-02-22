using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Сущность для представления показаний водомеров
/// </summary>
public class MeterReadingEntity
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор счетчика, которому принадлежит показание
    /// </summary>
    [Required]
    public Guid WaterMeterId { get; set; }
    
    /// <summary>
    /// Показание счетчика
    /// </summary>
    [Required]
    [StringLength(5, MinimumLength = 5)]
    public string WaterValue { get; set; } = "00000";

    /// <summary>
    /// Сумма показаний за все время
    /// </summary>
    [Required]
    public int TotalValue { get; set; }

    /// <summary>
    /// Разница сумм показаний между текущей и предыдущей записью
    /// </summary>
    [Required]
    public int DifferenceValue { get; set; }

    /// <summary>
    /// Дата, на которую было зафиксировано показание
    /// </summary>
    [Required]
    public DateTime ReadingDate { get; set; }

    /// <summary>
    /// Дата и время создания записи показания (UTC)
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Связанный счетчик (навигационное свойство)
    /// </summary>
    public WaterMeterEntity WaterMeter { get; set; } = null!;
}