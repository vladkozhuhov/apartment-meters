using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.Base;

/// <summary>
/// Реализация репозитория для работы с счетчиком
/// </summary>
public class WaterMeterRepository : IWaterMeterRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Конструктор с зависимостью от контекста базы данных
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public WaterMeterRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<WaterMeterEntity> AddAsync(WaterMeterEntity waterMeterEntity)
    {
        await _dbContext.WaterMeters.AddAsync(waterMeterEntity);
        await _dbContext.SaveChangesAsync();

        return waterMeterEntity;
    }

    /// <inheritdoc />
    public async Task<WaterMeterEntity?> GetByIdAsync(Guid id)
    {
        return await _dbContext.WaterMeters
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<WaterMeterEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.WaterMeters
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(WaterMeterEntity waterMeterEntity)
    {
        _dbContext.WaterMeters.Update(waterMeterEntity);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(WaterMeterEntity waterMeterEntity)
    {
        _dbContext.WaterMeters.Remove(waterMeterEntity);
        await _dbContext.SaveChangesAsync();
    }
}