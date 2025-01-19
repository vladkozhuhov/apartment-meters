using System.Net;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{
    private readonly IUserCommand _command;
    private readonly IUserQuery _query;
    
    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис для выполнения команд над пользователями</param>
    /// <param name="query">Сервис для выполнения запросов о пользователях</param>
    public UserController(IUserCommand command, IUserQuery query)
    {
        _command = command;
        _query = query;
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet("function/user-get/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _query.GetUserByIdAsync(id);
        return user != null ? Ok(user) : NotFound("User not found");
    }
    
    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet("function/users-get")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllUser()
    {
        var user = await _query.GetAllUsersAsync();
        return user != null ? Ok(user) : NotFound("User not found");
    }

    /// <summary>
    /// Добавить нового пользователя
    /// </summary> 
    /// <param name="addUserDto">Данные нового пользователя</param>
    /// <returns>Нового пользователя</returns>
    [HttpPost("function/user-add")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto addUserDto)
    {
        var createdUser = await _command.AddUserAsync(addUserDto);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="updateUserDto">Обновленные данные пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpPut("function/user-update/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        await _command.UpdateUserAsync(id, updateUserDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("function/user-delete/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _command.DeleteUserAsync(id);
        return NoContent();
    }
}