using System.Net;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Application.Models.MeterReadingModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с показаниями водомеров
/// </summary>
[Route("api/")]
[ApiController]
public class WaterMeterReadingController : ControllerBase
{
    private readonly IMeterReadingCommand _command;
    private readonly IMeterReadingQuery _query;

    public WaterMeterReadingController(IMeterReadingCommand command, IMeterReadingQuery query)
    {
        _command = command;
        _query = query;
    }

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    [HttpGet("function/meterReadings-get")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllWaterMeterReadings()
    {
        var readings = await _query.GetAllMeterReadingAsync();
        return Ok(readings);
    }
    
    /// <summary>
    /// Получить все показания водомеров по пользователю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    [HttpGet("function/meterReadingByUser-get/{userId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetMeterReadingsByUserId(Guid userId)
    {
        var readings = await _query.GetMeterReadingByUserIdAsync(userId);
        return Ok(readings);
    }
    
    /// <summary>
    /// Получить все показания водомеров по пользователю
    /// </summary>
    /// <param name="id">Идентификатор показания водомера</param>
    [HttpGet("function/meterReadingById-get/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetMeterReadingsById(Guid id)
    {
        var readings = await _query.GetMeterReadingByIdAsync(id);
        return Ok(readings);
    }

    /// <summary>
    /// Добавить новое показание
    /// </summary>
    /// <param name="request">Модель добавления показания</param>
    [HttpPost("function/meterReading-add")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddWaterMeterReading([FromBody] MeterReadingAddDto request)
    {
        await _command.AddMeterReadingAsync(request);
        return Ok("Показание добавлено");
    }
    
    /// <summary>
    /// Обновить данные показания водомеров
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="meterReadingUpdateDto">Обновленные данные показания водомера</param>
    [HttpPut("function/meterReading-update/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateWaterMeterReading(Guid id, [FromBody] MeterReadingUpdateDto meterReadingUpdateDto)
    {
        await _command.UpdateMeterReadingAsync(id, meterReadingUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    [HttpDelete("function/meterReading-delete/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteWaterMeterReading(Guid id)
    {
        await _command.DeleteMeterReadingAsync(id);
        return Ok("Показание удалено");
    }
}