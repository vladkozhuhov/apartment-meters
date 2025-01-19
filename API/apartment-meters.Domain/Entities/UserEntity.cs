using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Сущность для представления пользователя системы
/// </summary>
public class UserEntity
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    [Required]
    [Range(1, 999)]
    public int ApartmentNumber { get; set; }

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество пользователя (необязательное поле)
    /// </summary>
    [MaxLength(50)]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Номер телефона пользователя (необязательный параметр)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя в системе (например, пользователь или администратор)
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Заводской номер счетчика
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string FactoryNumber { get; set; } = null!;

    /// <summary>
    /// Год выпуска счетчика
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateTime FactoryYear { get; set; }

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
    public ICollection<MeterReadingEntity> MeterReadings { get; set; } = new List<MeterReadingEntity>();
}