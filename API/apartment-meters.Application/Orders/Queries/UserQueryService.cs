using Application.Interfaces.Queries;
using Application.Models;
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
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        // Получаем всех пользователей из репозитория
        var users = await _userRepository.GetAllAsync();

        // Преобразуем их в DTO
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            ApartmentNumber = user.ApartmentNumber,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        });
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Данные пользователя в формате UserDto</returns>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если пользователь не найден</exception>
    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        // Получаем пользователя по ID
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        // Преобразуем сущность User в DTO
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            ApartmentNumber = user.ApartmentNumber,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };
    }
}