using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Сущность для представления показаний водомеров
/// </summary>
public class MeterReadingEntity
{
    /// <summary>
    /// Уникальный идентификатор показания
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя, которому принадлежит показание
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Показание холодной воды для основного счетчика
    /// </summary>
    [Required]
    [StringLength(5, MinimumLength = 5)]
    public string PrimaryColdWaterValue { get; set; } = "00000";

    /// <summary>
    /// Показание горячей воды для основного счетчика
    /// </summary>
    [Required]
    [StringLength(5, MinimumLength = 5)]
    public string PrimaryHotWaterValue { get; set; } = "00000";

    /// <summary>
    /// Сумма показаний холодной и горячей воды для основного счетчика
    /// </summary>
    [Required]
    public int PrimaryTotalValue { get; set; }

    /// <summary>
    /// Разница сумм показаний между текущей и предыдущей записью для основного счетчика
    /// </summary>
    [Required]
    public int PrimaryDifferenceValue { get; set; }

    /// <summary>
    /// Флаг, указывающий, есть ли у пользователя дополнительный счетчик
    /// </summary>
    [Required]
    public bool HasSecondaryMeter { get; set; }

    /// <summary>
    /// Показание холодной воды для дополнительного счетчика
    /// </summary>
    [StringLength(5, MinimumLength = 5)]
    public string? SecondaryColdWaterValue { get; set; } = "00000";

    /// <summary>
    /// Показание горячей воды для дополнительного счетчика
    /// </summary>
    [StringLength(5, MinimumLength = 5)]
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
    /// Связанный пользователь (навигационное свойство)
    /// </summary>
    [Required]
    public UserEntity UserEntity { get; set; } = null!;
}