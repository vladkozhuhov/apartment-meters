using Domain.Entities;
using Domain.Repositories;

namespace Application.Services;

/// <summary>
/// Сервис для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingService
{
    private readonly IWaterMeterReadingRepository _repository;

    public WaterMeterReadingService(IWaterMeterReadingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Получить все показания водомеров
    /// </summary>
    /// <returns>Список показаний</returns>
    public async Task<IEnumerable<MeterReading>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Получить показания пользователя по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список показаний</returns>
    public async Task<IEnumerable<MeterReading>> GetByUserIdAsync(Guid userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    /// <summary>
    /// Добавить новое показание
    /// </summary>
    /// <param name="meterReading">Сущность показания</param>
    public async Task AddAsync(MeterReading meterReading)
    {
        await _repository.AddAsync(meterReading);
    }

    /// <summary>
    /// Удалить показание
    /// </summary>
    /// <param name="id">Идентификатор показания</param>
    public async Task DeleteAsync(Guid id)
    {
        var reading = await _repository.GetByIdAsync(id);
        if (reading != null)
        {
            await _repository.DeleteAsync(reading);
        }
    }
}