namespace Domain.Entities;

public class AuthenticationToken
{
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Навигационные свойства
    public User User { get; set; } = null!;
}