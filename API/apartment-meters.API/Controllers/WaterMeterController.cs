using System.Net;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models.WaterMeterModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы со счетчиками
/// </summary>
[Route("api/")]
[ApiController]
public class WaterMeterController : ControllerBase
{
    private readonly IWaterMeterCommand _command;
    private readonly IWaterMeterQuery _query;

    public WaterMeterController(IWaterMeterCommand command, IWaterMeterQuery query)
    {
        _command = command;
        _query = query;
    }
    
    /// <summary>
    /// Получить информацию о счетчике
    /// </summary>
    /// <param name="id">Идентификатор счетчика</param>
    [HttpGet("function/waterMeterById-get/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetWaterMeterById(Guid id)
    {
        var readings = await _query.GetWaterMeterByIdAsync(id);
        return Ok(readings);
    }
    
    /// <summary>
    /// Получить все счетчики по пользователю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    [HttpGet("function/waterMeterByUser-get/{userId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetWaterMetersByUserId(Guid userId)
    {
        var readings = await _query.GetWaterMeterByUserIdAsync(userId);
        return Ok(readings);
    }

    /// <summary>
    /// Добавить новое показание
    /// </summary>
    /// <param name="request">Модель добавления показания</param>
    [HttpPost("function/waterMeter-add")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddWaterMeterReading([FromBody] WaterMeterAddDto request)
    {
        await _command.AddWaterMeterAsync(request);
        return Ok("Счетчик добавлен");
    }
    
    /// <summary>
    /// Обновить данные показания водомеров
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="waterMeterUpdateDto">Обновленные данные счетчика</param>
    [HttpPut("function/waterMeter-update/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateWaterMeterReading(Guid id, [FromBody] WaterMeterUpdateDto waterMeterUpdateDto)
    {
        await _command.UpdateWaterMeterAsync(id, waterMeterUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Удалить счетчик
    /// </summary>
    /// <param name="id">Идентификатор счетчика</param>
    [HttpDelete("function/waterMeter-delete/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteWaterMeterReading(Guid id)
    {
        await _command.DeleteWaterMeterAsync(id);
        return Ok("Счетчик удален");
    }
}