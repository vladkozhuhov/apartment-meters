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
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество пользователя (необязательное поле)
    /// </summary>
    public string? MiddleName { get; set; }

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
    
    /// <summary>
    /// Заводской номер счетчика
    /// </summary>
    public string FactoryNumber { get; set; } = null!;

    /// <summary>
    /// Год выпуска счетчика
    /// </summary>
    public DateTime FactoryYear { get; set; }
}