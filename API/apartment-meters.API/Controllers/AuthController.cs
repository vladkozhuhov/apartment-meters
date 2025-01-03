using System.Net;
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var authenticatedUser = await _authService.LoginAsync(loginDto);

        if (authenticatedUser == null)
            return Unauthorized("Неверный номер квартиры или пароль.");

        return Ok(authenticatedUser);
    }
}