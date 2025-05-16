using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models.MeterReadingModel;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
public class MeterReadingController : ControllerBase
{
    private readonly IMeterReadingCommand _command;
    private readonly IMeterReadingQuery _query;
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly ILogger<MeterReadingController> _logger;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    /// <param name="command">Сервис команд для показаний счетчиков</param>
    /// <param name="query">Сервис запросов для показаний счетчиков</param>
    /// <param name="errorHandlingService">Сервис обработки ошибок</param>
    /// <param name="logger">Сервис логирования</param>
    public MeterReadingController(
        IMeterReadingCommand command, 
        IMeterReadingQuery query,
        IErrorHandlingService errorHandlingService,
        ILogger<MeterReadingController> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
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
        
        try
        {
            var readings = await _query.GetMeterReadingByWaterMeterIdAsync(waterMeterId);
            return Ok(readings);
        }
        catch(Exception)
        {
            _errorHandlingService.ThrowNotFoundException(ErrorType.WaterMeterNotFoundError351, 
                $"Счетчик с идентификатором {waterMeterId} не найден.");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        if (reading == null)
        {
            _errorHandlingService.ThrowNotFoundException(ErrorType.MeterReadingNotFoundError352, 
                $"Показание счетчика с идентификатором {id} не найдено.");
        }
        
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
        
        try
        {
            var createdReading = await _command.AddMeterReadingAsync(request);
            return CreatedAtAction(nameof(GetMeterReadingById), new { id = createdReading.Id }, createdReading);
        }
        catch (BusinessLogicException ex) when (ex.ErrorType == ErrorType.MeterReadingLessThanPreviousError353)
        {
            // Просто пробрасываем ошибку, фильтр исключений её перехватит и вернёт правильный ответ
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении показания счетчика");
            _errorHandlingService.ThrowBusinessLogicException(ErrorType.InvalidDataFormatError401, 
                "Произошла ошибка при добавлении показания счетчика.");
            return BadRequest(); // Эта строка не будет выполнена, нужна для компиляции
        }
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
        
        try
        {
            // Проверяем наличие обязательных полей в запросе
            if (meterReadingUpdateDto == null)
            {
                return BadRequest(new { message = "Тело запроса не может быть пустым" });
            }
            
            if (meterReadingUpdateDto.WaterMeterId == Guid.Empty)
            {
                return BadRequest(new { message = "Идентификатор водомера не может быть пустым" });
            }

            await _command.UpdateMeterReadingAsync(id, meterReadingUpdateDto);
            return NoContent();
        }
        catch (BusinessLogicException ex) when (ex.ErrorType == ErrorType.MeterReadingLessThanPreviousError353)
        {
            _logger.LogWarning("Попытка установить показание меньше предыдущего: {ErrorMessage}", ex.Message);
            return BadRequest(new { 
                errorType = ex.ErrorType,
                message = "Новое показание не может быть меньше предыдущего."
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Не найдено показание: {ErrorMessage}", ex.Message);
            return NotFound(new { 
                errorType = ErrorType.MeterReadingNotFoundError352,
                message = ex.Message
            });
        }
        catch (MeterReadingValidationException ex)
        {
            _logger.LogWarning("Ошибка валидации: {ErrorType}, {ErrorMessage}", ex.ErrorType, ex.Message);
            return BadRequest(new { 
                errorType = ex.ErrorType,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении показания счетчика {MeterReadingId}", id);
            return StatusCode(500, new {
                errorType = ErrorType.InvalidDataFormatError401,
                message = "Произошла ошибка при обновлении показания счетчика."
            });
        }
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
        
        try
        {
            await _command.DeleteMeterReadingAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _errorHandlingService.ThrowNotFoundException(ErrorType.MeterReadingNotFoundError352, 
                $"Показание счетчика с идентификатором {id} не найдено.");
            return NotFound(); // Эта строка не будет выполнена, нужна для компиляции
        }
    }
}