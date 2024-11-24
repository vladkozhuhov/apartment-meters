using Domain.Enums;

namespace Application.Models;

/// <summary>
/// DTO для создания нового пользователя
/// </summary>
public class AddUserDto
{
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
    /// Номер телефона пользователя
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public UserRole Role { get; set; }
}