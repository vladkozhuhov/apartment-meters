using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Интерфейс для сервиса фоновых задач
/// </summary>
public interface IBackgroundTaskService
{
    /// <summary>
    /// Запустить новую фоновую задачу
    /// </summary>
    /// <param name="name">Имя задачи</param>
    /// <param name="action">Действие, которое будет выполнено</param>
    /// <returns>DTO с информацией о созданной задаче</returns>
    BackgroundTaskDto StartNewTask(string name, Func<CancellationToken, Task> action);
    
    /// <summary>
    /// Получить информацию о задаче по идентификатору
    /// </summary>
    /// <param name="taskId">Идентификатор задачи</param>
    /// <returns>DTO с информацией о задаче или null, если задача не найдена</returns>
    BackgroundTaskDto? GetTaskInfo(Guid taskId);
    
    /// <summary>
    /// Получить информацию обо всех задачах
    /// </summary>
    /// <returns>Список DTO с информацией о задачах</returns>
    IEnumerable<BackgroundTaskDto> GetAllTasks();
    
    /// <summary>
    /// Отменить выполнение задачи
    /// </summary>
    /// <param name="taskId">Идентификатор задачи</param>
    /// <returns>true, если задача успешно отменена, false - если задача не найдена или уже завершена</returns>
    bool CancelTask(Guid taskId);
} 