using System.Net;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Application.Models.MeterReadingModel;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с показаниями водомеров
/// </summary>
[ApiController]
[Route("api/meter-readings")]
[Produces("application/json")]
[EnableRateLimiting("api-readings")]
public class MeterReadingController : ControllerBase
{
    private readonly IMeterReadingCommand _command;
    private readonly IMeterReadingQuery _query;
    private readonly ILogger<MeterReadingController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис команд для показаний счетчиков</param>
    /// <param name="query">Сервис запросов для показаний счетчиков</param>
    /// <param name="logger">Сервис логирования</param>
    public MeterReadingController(
        IMeterReadingCommand command, 
        IMeterReadingQuery query,
        ILogger<MeterReadingController> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Список всех показаний водомеров</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MeterReadingEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMeterReadings()
    {
        _logger.LogInformation("Запрос на получение всех показаний водомеров");
        var readings = await _query.GetAllMeterReadingAsync();
        return Ok(readings);
    }
    
    /// <summary>
    /// Получить все показания водомеров по идентификатору счетчика
    /// </summary>
    /// <param name="waterMeterId">Идентификатор счетчика</param>
    /// <returns>Список показаний для указанного счетчика</returns>
    [HttpGet("by-water-meter/{waterMeterId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<MeterReadingEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMeterReadingsByWaterMeterId(Guid waterMeterId)
    {
        _logger.LogInformation("Запрос на получение показаний для счетчика {WaterMeterId}", waterMeterId);
        var readings = await _query.GetMeterReadingByWaterMeterIdAsync(waterMeterId);
        return Ok(readings);
    }
    
    /// <summary>
    /// Получить показание водомера по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор показания водомера</param>
    /// <returns>Показание водомера</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MeterReadingEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMeterReadingById(Guid id)
    {
        _logger.LogInformation("Запрос на получение показания с ID {MeterReadingId}", id);
        var reading = await _query.GetMeterReadingByIdAsync(id);
        return Ok(reading);
    }

    /// <summary>
    /// Добавить новое показание
    /// </summary>
    /// <param name="request">Модель добавления показания</param>
    /// <returns>Результат операции</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMeterReading([FromBody] MeterReadingAddDto request)
    {
        _logger.LogInformation("Запрос на добавление нового показания для счетчика {WaterMeterId}", request.WaterMeterId);
        var createdReading = await _command.AddMeterReadingAsync(request);
        return CreatedAtAction(nameof(GetMeterReadingById), new { id = createdReading.Id }, createdReading);
    }
    
    /// <summary>
    /// Обновить данные показания водомера
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <param name="meterReadingUpdateDto">Обновленные данные показания водомера</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMeterReading(Guid id, [FromBody] MeterReadingUpdateDto meterReadingUpdateDto)
    {
        _logger.LogInformation("Запрос на обновление показания с ID {MeterReadingId}", id);
        await _command.UpdateMeterReadingAsync(id, meterReadingUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMeterReading(Guid id)
    {
        _logger.LogInformation("Запрос на удаление показания с ID {MeterReadingId}", id);
        await _command.DeleteMeterReadingAsync(id);
        return NoContent();
    }
}