using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
    /// Показание счетчика в формате "00000,000" (5 цифр до запятой и 3 после)
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^\d{1,5},\d{1,3}$", ErrorMessage = "Формат показаний должен содержать до 5 цифр до запятой и до 3 после")]
    public string WaterValue { get; set; } = "00000,000";

    /// <summary>
    /// Разница показаний между текущей и предыдущей записью
    /// </summary>
    [Required]
    public double DifferenceValue { get; set; }

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