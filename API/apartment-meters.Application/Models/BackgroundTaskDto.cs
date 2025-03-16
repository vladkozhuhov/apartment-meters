namespace Application.Models;

/// <summary>
/// DTO для работы с фоновыми задачами
/// </summary>
public class BackgroundTaskDto
{
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Имя задачи
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Текущий статус задачи
    /// </summary>
    public BackgroundTaskStatus Status { get; set; }
    
    /// <summary>
    /// Время создания задачи
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Время завершения задачи (если завершена)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Сообщение о результате выполнения (успех или ошибка)
    /// </summary>
    public string? ResultMessage { get; set; }
}

/// <summary>
/// Статус фоновой задачи
/// </summary>
public enum BackgroundTaskStatus
{
    /// <summary>
    /// Задача создана, но еще не запущена
    /// </summary>
    Created = 0,
    
    /// <summary>
    /// Задача запущена и выполняется
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// Задача успешно завершена
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Выполнение задачи завершилось с ошибкой
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Задача отменена пользователем
    /// </summary>
    Canceled = 4
} 