namespace Domain.Entities;

public class MeterReading
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal ColdWaterValue { get; set; }
    public decimal HotWaterValue { get; set; }
    public DateTime ReadingDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public User User { get; set; } = null!;
}