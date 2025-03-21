using System.Security.Claims;
using Application.Interfaces.Services;
using Application.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для авторизации пользователей в системе
/// </summary>
[Route("api/auth")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="authenticationService">Сервис аутентификации</param>
    /// <param name="logger">Сервис логирования</param>
    public AuthController(
        IAuthenticationService authenticationService,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Аутентификация пользователя по номеру квартиры и паролю
    /// </summary>
    /// <param name="request">Данные для входа (номер квартиры и пароль)</param>
    /// <returns>Информация о пользователе и JWT-токен для доступа к API</returns>
    /// <response code="200">Успешная аутентификация</response>
    /// <response code="401">Неверный номер квартиры или пароль</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Попытка входа пользователя с номером квартиры: {ApartmentNumber}", request.Username);
        
        var response = await _authenticationService.AuthenticateAsync(request.Username, request.Password);

        if (response == null)
        {
            _logger.LogWarning("Неудачная попытка входа с номером квартиры: {ApartmentNumber}", request.Username);
            return Unauthorized(new { message = "Неверный номер квартиры или пароль" });
        }

        _logger.LogInformation("Успешный вход пользователя с номером квартиры: {ApartmentNumber}", request.Username);
        return Ok(response);
    }

    /// <summary>
    /// Проверка текущего состояния аутентификации пользователя
    /// </summary>
    /// <returns>Данные о текущем авторизованном пользователе</returns>
    /// <response code="200">Информация о пользователе получена</response>
    /// <response code="401">Пользователь не авторизован</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> GetCurrentUser()
    {
        var userId = User.FindFirst("sub")?.Value;
        var username = User.FindFirst("name")?.Value;
        
        _logger.LogInformation("Запрос информации о пользователе. UserId: {UserId}", userId);

        return Ok(new
        {
            UserId = userId,
            Username = username,
            IsAuthenticated = User.Identity.IsAuthenticated,
            Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role)
                          .Select(c => c.Value)
                          .ToList()
        });
    }
}