using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models.WaterMeterModel;
using Application.Services;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
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
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly ILogger<WaterMeterController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис команд для счетчиков воды</param>
    /// <param name="query">Сервис запросов для счетчиков воды</param>
    /// <param name="errorHandlingService">Сервис обработки ошибок</param>
    /// <param name="logger">Сервис логирования</param>
    public WaterMeterController(
        IWaterMeterCommand command, 
        IWaterMeterQuery query,
        IErrorHandlingService errorHandlingService,
        ILogger<WaterMeterController> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
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
        
        if (waterMeter == null)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.WaterMeterNotFoundError351,
                $"Счетчик воды с ID {id} не найден");
        }
        
        return Ok(waterMeter);
    }
    
    /// <summary>
    /// Получить все счетчики воды по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список счетчиков воды пользователя</returns>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<WaterMeterEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWaterMetersByUserId(Guid userId)
    {
        _logger.LogInformation("Запрос на получение счетчиков воды для пользователя с ID {UserId}", userId);
        
        try
        {
            var waterMeters = await _query.GetWaterMeterByUserIdAsync(userId);
            return Ok(waterMeters);
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.UserDataNotFoundError201,
                $"Пользователь с ID {userId} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        
        try
        {
            var createdWaterMeter = await _command.AddWaterMeterAsync(request);
            return CreatedAtAction(nameof(GetWaterMeterById), new { id = createdWaterMeter.Id }, createdWaterMeter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении счетчика воды");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.InvalidDataFormatError401,
                "Произошла ошибка при добавлении счетчика воды");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        
        try
        {
            await _command.UpdateWaterMeterAsync(id, waterMeterUpdateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.WaterMeterNotFoundError351,
                $"Счетчик воды с ID {id} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении счетчика воды");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.InvalidDataFormatError401,
                "Произошла ошибка при обновлении счетчика воды");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        
        try
        {
            await _command.DeleteWaterMeterAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(
                ErrorType.WaterMeterNotFoundError351,
                $"Счетчик воды с ID {id} не найден");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении счетчика воды");
            _errorHandlingService.ThrowBusinessLogicException(
                ErrorType.InvalidDataFormatError401,
                "Произошла ошибка при удалении счетчика воды");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }
}