using System.Security.Claims;
using Application.Interfaces.Services;
using Application.Models.Auth;
using Application.Services;
using Application.Exceptions;
using Domain.Enums;
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
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="authenticationService">Сервис аутентификации</param>
    /// <param name="errorHandlingService">Сервис обработки ошибок</param>
    /// <param name="logger">Сервис логирования</param>
    public AuthController(
        IAuthenticationService authenticationService,
        IErrorHandlingService errorHandlingService,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
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
        
        try
        {
            var response = await _authenticationService.AuthenticateAsync(request.Username, request.Password);

            if (response == null)
            {
                _logger.LogWarning("Неудачная попытка входа с номером квартиры: {ApartmentNumber}", request.Username);
                _errorHandlingService.ThrowBusinessLogicException(
                    ErrorType.InvalidPasswordError102,
                    "Неверный номер квартиры или пароль");
            }

            _logger.LogInformation("Успешный вход пользователя с номером квартиры: {ApartmentNumber}", request.Username);
            return Ok(response);
        }
        catch (Exception ex) when (!(ex is BusinessLogicException))
        {
            _logger.LogError(ex, "Ошибка при попытке входа пользователя с номером квартиры: {ApartmentNumber}", request.Username);
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.UserBlockedError103,
                "Произошла ошибка при попытке входа");
            return Unauthorized(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var username = User.FindFirst("name")?.Value;
            
            if (User.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
            {
                _errorHandlingService.ThrowBusinessLogicException(
                    ErrorType.InvalidTokenError104,
                    "Пользователь не авторизован");
            }
            
            _logger.LogInformation("Запрос информации о пользователе. UserId: {UserId}", userId);

            return Ok(new
            {
                UserId = userId,
                Username = username,
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role)
                              .Select(c => c.Value)
                              .ToList()
            });
        }
        catch (Exception ex) when (!(ex is BusinessLogicException))
        {
            _logger.LogError(ex, "Ошибка при получении информации о текущем пользователе");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.UserBlockedError103,
                "Произошла ошибка при получении информации о пользователе");
            return Unauthorized(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }
}