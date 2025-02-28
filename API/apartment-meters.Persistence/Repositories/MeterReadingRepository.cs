using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

/// <summary>
/// Реализация репозитория для работы с показаниями водомеров
/// </summary>
public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Конструктор с зависимостью от контекста базы данных
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public MeterReadingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<MeterReadingEntity> AddAsync(MeterReadingEntity meterReadingEntity)
    {
        await _dbContext.MeterReadings.AddAsync(meterReadingEntity);
        await _dbContext.SaveChangesAsync();

        return meterReadingEntity;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetAllAsync()
    {
        return await _dbContext.MeterReadings.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<MeterReadingEntity?> GetByIdAsync(Guid id)
    {
        return await _dbContext.MeterReadings
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<MeterReadingEntity>> GetByWaterMeterIdAsync(Guid waterMeterId)
    {
        return await _dbContext.MeterReadings
            .Where(x => x.WaterMeterId == waterMeterId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(MeterReadingEntity meterReadingEntity)
    {
        _dbContext.MeterReadings.Remove(meterReadingEntity);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(MeterReadingEntity meterReadingEntity)
    {
        _dbContext.MeterReadings.Remove(meterReadingEntity);
        await _dbContext.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task<MeterReadingEntity?> GetLastByWaterMeterIdAsync(Guid waterMeterId)
    {
        return await _dbContext.MeterReadings
            .Where(m => m.WaterMeterId == waterMeterId)
            .OrderByDescending(m => m.ReadingDate)
            .FirstOrDefaultAsync();
    }
}