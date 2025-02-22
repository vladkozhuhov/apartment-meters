using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.UsersModel;

/// <summary>
/// DTO для обновления данных пользователя
/// </summary>
public class UserUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    [Required]
    public int? ApartmentNumber { get; set; }

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    [Required]
    public string? LastName { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    public string? FirstName { get; set; }

    /// <summary>
    /// Отчество пользователя (необязательное поле)
    /// </summary>
    [Required]
    public string? MiddleName { get; set; }
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required]
    public string? Password { get; set; } 

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    [Required]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    [Required]
    public UserRole? Role { get; set; }
}