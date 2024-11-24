using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с показаниями водомеров
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class WaterMeterReadingController : ControllerBase
{
    private readonly IWaterMeterReadingCommandService _commandService;
    private readonly IWaterMeterReadingQueryService _queryService;

    public WaterMeterReadingController(IWaterMeterReadingCommandService commandService, IWaterMeterReadingQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Список показаний</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var readings = await _queryService.GetAllMeterReadingsAsync();
        return Ok(readings);
    }

    /// <summary>
    /// Добавить новое показание
    /// </summary>
    /// <param name="request">Модель добавления показания</param>
    /// <returns>Результат операции</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWaterMeterReadingDto request)
    {
        await _commandService.AddMeterReadingAsync(request);
        return Ok("Показание добавлено");
    }

    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _commandService.DeleteMeterReadingAsync(id);
        return Ok("Показание удалено");
    }
}