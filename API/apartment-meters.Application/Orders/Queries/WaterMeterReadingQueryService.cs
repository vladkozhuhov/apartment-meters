using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Реализация сервиса запросов для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingQueryService : IWaterMeterReadingQueryService
{
    private readonly IWaterMeterReadingRepository _repository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="repository">Репозиторий показаний водомеров</param>
    public WaterMeterReadingQueryService(IWaterMeterReadingRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MeterReading>> GetAllMeterReadingsAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<MeterReading?> GetMeterReadingByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}