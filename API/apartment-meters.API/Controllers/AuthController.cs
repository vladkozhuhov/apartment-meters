using System.Net;
using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Models;
using Application.Models.LoginModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для авторизации пользователей
/// </summary>
[Route("api/")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="authService">Сервис аутентификации</param>
    /// <param name="logger">Сервис логирования</param>
    public AuthController(
        IAuthenticationService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="loginDto">Данные для входа</param>
    /// <returns>Информация о пользователе при успешной авторизации</returns>
    [ProducesResponseType(typeof(Domain.Entities.UserEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("function/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Запрос на авторизацию пользователя из квартиры {ApartmentNumber}", loginDto.ApartmentNumber);
            var authenticatedUser = await _authService.LoginAsync(loginDto);
            return Ok(authenticatedUser);
        }
        catch (CustomException ex)
        {
            _logger.LogWarning("Ошибка авторизации: {ErrorMessage}", ex.Message);
            return BadRequest(new ErrorResponse { Code = (int)ex.ErrorType, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Внутренняя ошибка сервера при авторизации пользователя");
            return StatusCode(500, new ErrorResponse { Code = 501, Message = "Внутренняя ошибка сервера" });
        }
    }
}