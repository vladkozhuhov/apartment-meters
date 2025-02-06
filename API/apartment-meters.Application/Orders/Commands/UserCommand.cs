using Application.Interfaces.Commands;
using Application.Models;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Сервис для выполнения команд, связанных с пользователями.
/// </summary>
public class UserCommand : IUserCommand
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор сервиса команд для пользователей.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public UserCommand(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<UserEntity> AddUserAsync(AddUserDto dto)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            ApartmentNumber = dto.ApartmentNumber,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            FactoryNumber = dto.FactoryNumber,
            FactoryYear = dto.FactoryYear,
        };

        await _userRepository.AddAsync(user);
        return user;
    }
    
    /// <inheritdoc />
    public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.LastName = dto.LastName ?? user.LastName;
        user.FirstName = dto.FirstName ?? user.FirstName;
        user.MiddleName = dto.MiddleName ?? user.MiddleName;
        user.ApartmentNumber = dto.ApartmentNumber ?? user.ApartmentNumber;
        user.FactoryNumber = dto.FactoryNumber ?? user.FactoryNumber;
        user.FactoryYear = dto.FactoryYear ?? user.FactoryYear;
        user.Password = dto.Password ?? user.Password;
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
        user.Role = dto.Role ?? user.Role;

        await _userRepository.UpdateAsync(user);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        await _userRepository.DeleteAsync(user);
    }
}