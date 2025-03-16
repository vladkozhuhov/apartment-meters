using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Queries;

/// <summary>
/// Сервис для выполнения операций чтения пользователей
/// </summary>
public class UserQuery : IUserQuery
{
    private readonly IUserRepository _userRepository;
    private readonly ICachedRepository<UserEntity, Guid> _cachedRepository;
    private readonly ILogger<UserQuery> _logger;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="userRepository">Репозиторий для работы с сущностью User</param>
    /// <param name="cachedRepository">Кэширующий репозиторий пользователей</param>
    /// <param name="logger">Сервис логирования</param>
    public UserQuery(
        IUserRepository userRepository,
        ICachedRepository<UserEntity, Guid> cachedRepository,
        ILogger<UserQuery> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _cachedRepository = cachedRepository ?? throw new ArgumentNullException(nameof(cachedRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<UserEntity> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Получение пользователя с ID {UserId}", id);
        var user = await _cachedRepository.GetByIdCachedAsync(id) ??
                  await _userRepository.GetByIdAsync(id);
                  
        if (user == null)
        {
            throw new KeyNotFoundException($"Пользователь с ID {id} не найден");
        }
        
        return user;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        _logger.LogInformation("Получение всех пользователей");
        return await _cachedRepository.GetAllCachedAsync();
    }
}