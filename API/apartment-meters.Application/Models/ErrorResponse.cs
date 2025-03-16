namespace Application.Models;

/// <summary>
/// Стандартизированный ответ с ошибкой
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = string.Empty;
} 