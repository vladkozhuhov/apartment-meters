using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Сущность для представления пользователя системы
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    public int ApartmentNumber { get; set; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Номер телефона пользователя (необязательный параметр)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя в системе (например, пользователь или администратор)
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Дата и время создания учетной записи пользователя (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата и время последнего обновления учетной записи пользователя (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Показания водомеров, связанные с пользователем.
    /// </summary>
    public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
}