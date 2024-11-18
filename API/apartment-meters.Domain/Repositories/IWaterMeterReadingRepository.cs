using Domain.Entities;

namespace Domain.Repositories;

public interface IWaterMeterReadingRepository
{
    Task<List<WaterMeterReading>> GetByUserIdAsync(int userId);
    Task<List<WaterMeterReading>> GetAllAsync();
    Task AddAsync(WaterMeterReading reading);
}