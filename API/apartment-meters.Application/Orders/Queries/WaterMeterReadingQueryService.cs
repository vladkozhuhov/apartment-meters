using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Реализация сервиса запросов для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingQueryService : IWaterMeterReadingQueryService
{
    private readonly IWaterMeterReadingRepository _waterMeterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingQueryService(IWaterMeterReadingRepository waterMeterReadingRepository)
    {
        _waterMeterReadingRepository = waterMeterReadingRepository;
    }

    /// <inheritdoc />
    public async Task<MeterReading?> GetMeterReadingByUserIdAsync(Guid userId)
    {
        return await _waterMeterReadingRepository.GetByIdAsync(userId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReading>> GetAllMeterReadingAsync()
    {
        return await _waterMeterReadingRepository.GetAllAsync();
    }
}