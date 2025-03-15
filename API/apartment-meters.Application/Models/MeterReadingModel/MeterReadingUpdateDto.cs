using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
    /// Показание счетчика (будет отформатировано в формат "00000,000")
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^\d{1,5},\d{1,3}$", ErrorMessage = "Формат показаний должен содержать до 5 цифр до запятой и до 3 после")]
    public string WaterValue { get; set; } = "00000,000";

    /// <summary>
    /// Дата, на которую было зафиксировано показание
    /// </summary>
    [Required]
    public DateTime ReadingDate { get; set; }
}