using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

/// <summary>
/// Реализация репозитория для работы с показаниями водомеров
/// </summary>
public class WaterMeterReadingRepository : IWaterMeterReadingRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Конструктор с зависимостью от контекста базы данных
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public WaterMeterReadingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<MeterReading> AddAsync(MeterReading meterReading)
    {
        await _dbContext.MeterReadings.AddAsync(meterReading);
        await _dbContext.SaveChangesAsync();

        return meterReading;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReading>> GetAllAsync()
    {
        return await _dbContext.MeterReadings.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<MeterReading?> GetByIdAsync(Guid id)
    {
        return await _dbContext.MeterReadings
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(MeterReading meterReading)
    {
        _dbContext.MeterReadings.Remove(meterReading);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(MeterReading meterReading)
    {
        _dbContext.MeterReadings.Remove(meterReading);
        await _dbContext.SaveChangesAsync();
    }
}