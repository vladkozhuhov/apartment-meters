using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models.UsersModel;
using Application.Services;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserCommand _command;
    private readonly IUserQuery _query;
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly ILogger<UserController> _logger;
    
    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис для выполнения команд над пользователями</param>
    /// <param name="query">Сервис для выполнения запросов о пользователях</param>
    /// <param name="errorHandlingService">Сервис обработки ошибок</param>
    /// <param name="logger">Сервис логирования</param>
    public UserController(
        IUserCommand command, 
        IUserQuery query,
        IErrorHandlingService errorHandlingService,
        ILogger<UserController> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Информация о пользователе</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,User")] // Разрешаем доступ для админов и обычных пользователей
    [ProducesResponseType(typeof(UserEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        _logger.LogInformation("Запрос на получение пользователя с ID {UserId}", id);
        var user = await _query.GetUserByIdAsync(id);
        
        if (user == null)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.UserDataNotFoundError201,
                $"Пользователь с ID {id} не найден");
        }
        
        return Ok(user);
    }
    
    /// <summary>
    /// Получить пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Информация о пользователе</returns>
    [HttpGet("by-apartment/{apartmentNumber}")]
    [Authorize(Roles = "Admin,User")] // Разрешаем доступ для админов и обычных пользователей
    [ProducesResponseType(typeof(UserEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserByApartmentNumber(string apartmentNumber)
    {
        _logger.LogInformation("Запрос на получение пользователя с номером квартиры {ApartmentNumber}", apartmentNumber);
        
        if (!int.TryParse(apartmentNumber, out int apartmentNumberInt))
        {
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.InvalidApartmentNumberError451,
                "Номер квартиры должен быть числом");
        }
        
        try
        {
            var user = await _query.GetUserByApartmentNumberAsync(apartmentNumberInt);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.UserNotFoundError101,
                $"Пользователь с номером квартиры {apartmentNumber} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }
    
    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")] // Только админы могут получить список всех пользователей
    [ProducesResponseType(typeof(IEnumerable<UserEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("Запрос на получение всех пользователей");
        var users = await _query.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Добавить нового пользователя
    /// </summary> 
    /// <param name="userAddDto">Данные нового пользователя</param>
    /// <returns>Информация о созданном пользователе</returns>
    [HttpPost]
    [AllowAnonymous] // Оставляем возможность создавать пользователей без авторизации для тестирования
    [ProducesResponseType(typeof(UserEntity), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUser([FromBody] UserAddDto userAddDto)
    {
        _logger.LogInformation("Запрос на создание нового пользователя");
        
        try
        {
            var createdUser = await _command.AddUserAsync(userAddDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании пользователя");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.InvalidDataFormatError401,
                "Произошла ошибка при создании пользователя");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="userUpdateDto">Обновленные данные пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")] // Только админы могут обновлять данные пользователей
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userUpdateDto)
    {
        _logger.LogInformation("Запрос на обновление пользователя с ID {UserId}", id);
        
        try
        {
            await _command.UpdateUserAsync(id, userUpdateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.UserDataNotFoundError201,
                $"Пользователь с ID {id} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении пользователя");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.UserUpdateFailedError202,
                "Произошла ошибка при обновлении пользователя");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }

    /// <summary>
    /// Удалить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")] // Только админы могут удалять пользователей
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger.LogInformation("Запрос на удаление пользователя с ID {UserId}", id);
        
        try
        {
            await _command.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.UserDataNotFoundError201,
                $"Пользователь с ID {id} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении пользователя");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.UserDeletionFailedError302,
                "Невозможно удалить данные по пользователю");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }
}