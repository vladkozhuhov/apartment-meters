using System.ComponentModel.DataAnnotations;

namespace Application.Models.MeterReadingModel;

/// <summary>
/// DTO для обновления показания водомера
/// </summary>
public class MeterReadingUpdateDto
{
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
    /// Дата, на которую было зафиксировано показание
    /// </summary>
    [Required]
    public DateTime ReadingDate { get; set; }
}