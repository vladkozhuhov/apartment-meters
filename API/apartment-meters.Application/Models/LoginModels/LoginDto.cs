namespace Application.Models.LoginModels;

/// <summary>
/// DTO для входа пользователя в систему
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Номер квартиры пользователя
    /// </summary>
    public int ApartmentNumber { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = null!;
}