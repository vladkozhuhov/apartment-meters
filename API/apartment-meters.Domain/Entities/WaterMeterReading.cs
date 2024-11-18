namespace Domain.Entities;

public class WaterMeterReading
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime Date { get; set; }
    public int ColdWaterReading { get; set; }
    public int HotWaterReading { get; set; }
}