using Domain.Enums;

namespace Application.Models;

/// <summary>
/// DTO для обновления данных пользователя
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    public int? ApartmentNumber { get; set; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName { get; set; } = null!;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string? Password { get; set; } 

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public UserRole? Role { get; set; }
}