using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с подписками на push-уведомления
/// </summary>
public class PushSubscriptionRepository : IPushSubscriptionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PushSubscriptionRepository> _logger;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Логгер</param>
    public PushSubscriptionRepository(ApplicationDbContext context, ILogger<PushSubscriptionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PushSubscriptionEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _context.PushSubscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<PushSubscriptionEntity> GetByIdAsync(Guid id)
    {
        return await _context.PushSubscriptions
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <inheritdoc />
    public async Task<PushSubscriptionEntity> GetByEndpointAsync(string endpoint)
    {
        return await _context.PushSubscriptions
            .FirstOrDefaultAsync(s => s.Endpoint == endpoint);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PushSubscriptionEntity>> GetAllActiveAsync()
    {
        return await _context.PushSubscriptions
            .Include(s => s.User)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(PushSubscriptionEntity subscription)
    {
        try
        {
            await _context.PushSubscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении подписки на push-уведомления");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PushSubscriptionEntity subscription)
    {
        try
        {
            _context.PushSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении подписки на push-уведомления");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var subscription = await GetByIdAsync(id);
            if (subscription is not null)
            {
                _context.PushSubscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении подписки на push-уведомления");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllForUserAsync(Guid userId)
    {
        try
        {
            var subscriptions = await GetByUserIdAsync(userId);
            if (subscriptions.Any())
            {
                _context.PushSubscriptions.RemoveRange(subscriptions);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении всех подписок пользователя на push-уведомления");
            throw;
        }
    }
} 