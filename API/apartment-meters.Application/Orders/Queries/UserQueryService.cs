using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Queries;

/// <summary>
/// Сервис для выполнения операций чтения пользователей
/// </summary>
public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="userRepository">Репозиторий для работы с сущностью User</param>
    public UserQueryService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    /// <inheritdoc />
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }
}