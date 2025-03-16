using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Queries;

/// <summary>
/// Сервис для выполнения операций чтения счетчика
/// </summary>
public class WaterMeterQuery : IWaterMeterQuery
{
    private readonly IWaterMeterRepository _waterMeterRepository;
    private readonly ICachedRepository<WaterMeterEntity, Guid> _cachedRepository;
    private readonly ILogger<WaterMeterQuery> _logger;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="waterMeterRepository">Репозиторий для работы с сущностью WaterMeter</param>
    /// <param name="cachedRepository">Кэширующий репозиторий счетчиков воды</param>
    /// <param name="logger">Сервис логирования</param>
    public WaterMeterQuery(
        IWaterMeterRepository waterMeterRepository,
        ICachedRepository<WaterMeterEntity, Guid> cachedRepository,
        ILogger<WaterMeterQuery> logger)
    {
        _waterMeterRepository = waterMeterRepository ?? throw new ArgumentNullException(nameof(waterMeterRepository));
        _cachedRepository = cachedRepository ?? throw new ArgumentNullException(nameof(cachedRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<WaterMeterEntity>> GetWaterMeterByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Получение счетчиков воды для пользователя с ID {UserId}", userId);
        return await _waterMeterRepository.GetByUserIdAsync(userId);
    }
    
    /// <inheritdoc />
    public async Task<WaterMeterEntity> GetWaterMeterByIdAsync(Guid id)
    {
        _logger.LogInformation("Получение счетчика воды с ID {WaterMeterId}", id);
        return await _cachedRepository.GetByIdCachedAsync(id) ??
               await _waterMeterRepository.GetByIdAsync(id);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<WaterMeterEntity>> GetAllWaterMetersAsync()
    {
        _logger.LogInformation("Получение всех счетчиков воды");
        return await _cachedRepository.GetAllCachedAsync();
    }
}