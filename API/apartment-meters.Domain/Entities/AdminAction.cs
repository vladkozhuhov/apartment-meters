namespace Domain.Entities;

public class AdminAction
{
    /// <summary>
    /// Идентификатор груза
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор груза
    /// </summary>
    public Guid AdminId { get; set; }
    
    /// <summary>
    /// Идентификатор груза
    /// </summary>
    public string ActionType { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор груза
    /// </summary>
    public string ActionDetails { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор груза
    /// </summary>
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// навигационное свойство
    /// </summary>
    public User Admin { get; set; } = null!;
}