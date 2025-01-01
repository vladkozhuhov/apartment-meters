using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserCommandService _commandService;
    private readonly IUserQueryService _queryService;
    
    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="commandService">Сервис для выполнения команд над пользователями</param>
    /// <param name="queryService">Сервис для выполнения запросов о пользователях</param>
    public UserController(IUserCommandService commandService, IUserQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _queryService.GetUserByIdAsync(id);
        return user != null ? Ok(user) : NotFound("User not found");
    }
    
    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUser()
    {
        var user = await _queryService.GetAllUsersAsync();
        return user != null ? Ok(user) : NotFound("User not found");
    }

    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="addUserDto">Данные нового пользователя</param>
    /// <returns>Нового пользователя</returns>
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto addUserDto)
    {
        await _commandService.AddUserAsync(addUserDto);
        return CreatedAtAction(nameof(GetUserById), new { id = addUserDto.FullName }, addUserDto);
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="updateUserDto">Обновленные данные пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        await _commandService.UpdateUserAsync(updateUserDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _commandService.DeleteUserAsync(id);
        return NoContent();
    }
}