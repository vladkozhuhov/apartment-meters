namespace Domain.Entities;

/// <summary>
/// Сущность для представления действий администратора
/// </summary>
public class AdminAction
{
    /// <summary>
    /// Уникальный идентификатор действия
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор администратора, совершившего действие
    /// </summary>
    public Guid AdminId { get; set; }

    /// <summary>
    /// Тип действия (например, "Создание пользователя")
    /// </summary>
    public string ActionType { get; set; } = null!;

    /// <summary>
    /// Детали действия (дополнительная информация о выполненной операции)
    /// </summary>
    public string ActionDetails { get; set; } = null!;

    /// <summary>
    /// Дата и время совершения действия (UTC)
    /// </summary>
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Связанный администратор (навигационное свойство)
    /// </summary>
    public UserEntity Admin { get; set; } = null!;
}