using Application.Interfaces.Commands;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="loginDto">Данные для входа</param>
    /// <returns>Результат авторизации</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var isAuthenticated = await _authService.LoginAsync(loginDto);

        if (!isAuthenticated)
            return Unauthorized("Неверный номер квартиры или пароль.");

        return Ok("Вы успешно авторизовались.");
    }
}