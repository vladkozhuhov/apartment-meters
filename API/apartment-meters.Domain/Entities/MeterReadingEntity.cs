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
    /// Показание холодной воды
    /// </summary>
    public decimal ColdWaterValue { get; set; }

    /// <summary>
    /// Показание горячей воды
    /// </summary>
    public decimal HotWaterValue { get; set; }

    /// <summary>
    /// Дата, на которую было зафиксировано показание
    /// </summary>
    public DateTime ReadingDate { get; set; }

    /// <summary>
    /// Дата и время создания записи показания (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Связанный пользователь (навигационное свойство)
    /// </summary>
    public UserEntity UserEntity { get; set; } = null!;
}