using Application.Interfaces.Services;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Infrastructure.Services;

/// <summary>
/// Реализация сервиса фоновых задач
/// </summary>
public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly ConcurrentDictionary<Guid, BackgroundTaskInfo> _tasks = new();
    private readonly ILogger<BackgroundTaskService> _logger;
    
    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="logger">Сервис логирования</param>
    public BackgroundTaskService(ILogger<BackgroundTaskService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public BackgroundTaskDto StartNewTask(string name, Func<CancellationToken, Task> action)
    {
        var taskId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        
        var taskInfo = new BackgroundTaskInfo
        {
            Id = taskId,
            Name = name,
            Status = BackgroundTaskStatus.Created,
            CreatedAt = DateTime.UtcNow,
            CancellationTokenSource = cts
        };
        
        _tasks[taskId] = taskInfo;
        
        _logger.LogInformation("Создана новая фоновая задача: {TaskName} с ID {TaskId}", name, taskId);
        
        // Запускаем задачу асинхронно
        Task.Run(async () =>
        {
            try
            {
                // Обновляем статус на Running
                taskInfo.Status = BackgroundTaskStatus.Running;
                _logger.LogInformation("Запущена фоновая задача: {TaskName} с ID {TaskId}", name, taskId);
                
                // Выполняем действие
                await action(cts.Token);
                
                // Если задача не была отменена, отмечаем её как успешно завершенную
                if (!cts.IsCancellationRequested)
                {
                    taskInfo.Status = BackgroundTaskStatus.Completed;
                    taskInfo.CompletedAt = DateTime.UtcNow;
                    taskInfo.ResultMessage = "Задача успешно выполнена";
                    _logger.LogInformation("Фоновая задача успешно завершена: {TaskName} с ID {TaskId}", name, taskId);
                }
            }
            catch (OperationCanceledException)
            {
                taskInfo.Status = BackgroundTaskStatus.Canceled;
                taskInfo.CompletedAt = DateTime.UtcNow;
                taskInfo.ResultMessage = "Задача была отменена";
                _logger.LogInformation("Фоновая задача отменена: {TaskName} с ID {TaskId}", name, taskId);
            }
            catch (Exception ex)
            {
                taskInfo.Status = BackgroundTaskStatus.Failed;
                taskInfo.CompletedAt = DateTime.UtcNow;
                taskInfo.ResultMessage = $"Ошибка: {ex.Message}";
                _logger.LogError(ex, "Ошибка при выполнении фоновой задачи: {TaskName} с ID {TaskId}", name, taskId);
            }
        });
        
        return MapToDto(taskInfo);
    }
    
    /// <inheritdoc />
    public BackgroundTaskDto? GetTaskInfo(Guid taskId)
    {
        if (_tasks.TryGetValue(taskId, out var taskInfo))
        {
            return MapToDto(taskInfo);
        }
        
        return null;
    }
    
    /// <inheritdoc />
    public IEnumerable<BackgroundTaskDto> GetAllTasks()
    {
        return _tasks.Values.Select(MapToDto).ToList();
    }
    
    /// <inheritdoc />
    public bool CancelTask(Guid taskId)
    {
        if (_tasks.TryGetValue(taskId, out var taskInfo))
        {
            if (taskInfo.Status == BackgroundTaskStatus.Running || taskInfo.Status == BackgroundTaskStatus.Created)
            {
                try
                {
                    taskInfo.CancellationTokenSource.Cancel();
                    _logger.LogInformation("Фоновая задача будет отменена: {TaskName} с ID {TaskId}", taskInfo.Name, taskId);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при отмене фоновой задачи: {TaskName} с ID {TaskId}", taskInfo.Name, taskId);
                    return false;
                }
            }
        }
        
        return false;
    }
    
    private static BackgroundTaskDto MapToDto(BackgroundTaskInfo taskInfo)
    {
        return new BackgroundTaskDto
        {
            Id = taskInfo.Id,
            Name = taskInfo.Name,
            Status = taskInfo.Status,
            CreatedAt = taskInfo.CreatedAt,
            CompletedAt = taskInfo.CompletedAt,
            ResultMessage = taskInfo.ResultMessage
        };
    }
}

/// <summary>
/// Внутренний класс для хранения информации о фоновой задаче
/// </summary>
internal class BackgroundTaskInfo
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
    
    /// <summary>
    /// Источник токена отмены для возможности отмены задачи
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
} 