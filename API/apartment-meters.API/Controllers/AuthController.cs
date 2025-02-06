using System.Net;
using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Models;
using Application.Models.Login;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/")]
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
    [HttpPost("function/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var authenticatedUser = await _authService.LoginAsync(loginDto);
            return Ok(authenticatedUser);
        }
        catch (CustomException ex)
        {
            return BadRequest(new { Code = (int)ex.ErrorType, Message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Code = 501, Message = "Внутренняя ошибка сервера" });
        }
    }
}