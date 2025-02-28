using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.UsersModel;

/// <summary>
/// DTO для создания нового пользователя с счетчиками
/// </summary>
public class UserAddDto
{
    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    [Required]
    public int ApartmentNumber { get; set; }

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    [Required]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество пользователя (необязательное поле)
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    [Required]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
}