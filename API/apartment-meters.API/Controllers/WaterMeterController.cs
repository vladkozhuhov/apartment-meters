using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models.WaterMeterModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы со счетчиками воды
/// </summary>
[ApiController]
[Route("api/water-meters")]
[Produces("application/json")]
[Authorize]
public class WaterMeterController : ControllerBase
{
    private readonly IWaterMeterCommand _command;
    private readonly IWaterMeterQuery _query;
    private readonly ILogger<WaterMeterController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис команд для счетчиков воды</param>
    /// <param name="query">Сервис запросов для счетчиков воды</param>
    /// <param name="logger">Сервис логирования</param>
    public WaterMeterController(
        IWaterMeterCommand command, 
        IWaterMeterQuery query,
        ILogger<WaterMeterController> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Получить информацию о счетчике воды по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор счетчика</param>
    /// <returns>Информация о счетчике воды</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WaterMeterEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWaterMeterById(Guid id)
    {
        _logger.LogInformation("Запрос на получение счетчика воды с ID {WaterMeterId}", id);
        var waterMeter = await _query.GetWaterMeterByIdAsync(id);
        return waterMeter != null ? Ok(waterMeter) : NotFound($"Счетчик воды с ID {id} не найден");
    }
    
    /// <summary>
    /// Получить все счетчики воды по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список счетчиков воды пользователя</returns>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<WaterMeterEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWaterMetersByUserId(Guid userId)
    {
        _logger.LogInformation("Запрос на получение счетчиков воды для пользователя с ID {UserId}", userId);
        var waterMeters = await _query.GetWaterMeterByUserIdAsync(userId);
        return Ok(waterMeters);
    }

    /// <summary>
    /// Добавить новый счетчик воды
    /// </summary>
    /// <param name="request">Модель добавления счетчика воды</param>
    /// <returns>Результат операции</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddWaterMeter([FromBody] WaterMeterAddDto request)
    {
        _logger.LogInformation("Запрос на добавление нового счетчика воды");
        var createdWaterMeter = await _command.AddWaterMeterAsync(request);
        return CreatedAtAction(nameof(GetWaterMeterById), new { id = createdWaterMeter.Id }, createdWaterMeter);
    }
    
    /// <summary>
    /// Обновить данные счетчика воды
    /// </summary>
    /// <param name="id">Идентификатор счетчика воды</param>
    /// <param name="waterMeterUpdateDto">Обновленные данные счетчика воды</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateWaterMeter(Guid id, [FromBody] WaterMeterUpdateDto waterMeterUpdateDto)
    {
        _logger.LogInformation("Запрос на обновление счетчика воды с ID {WaterMeterId}", id);
        await _command.UpdateWaterMeterAsync(id, waterMeterUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить счетчик воды
    /// </summary>
    /// <param name="id">Идентификатор счетчика воды</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteWaterMeter(Guid id)
    {
        _logger.LogInformation("Запрос на удаление счетчика воды с ID {WaterMeterId}", id);
        await _command.DeleteWaterMeterAsync(id);
        return NoContent();
    }
}