using Application.Interfaces.Commands;
using Application.Models;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands;

/// <summary>
/// Сервис для выполнения команд, связанных с пользователями.
/// </summary>
public class UserCommandService : IUserCommandService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор сервиса команд для пользователей.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public UserCommandService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<Guid> AddUserAsync(AddUserDto dto)
    {
        var user = new User
        {
            FullName = dto.FullName,
            ApartmentNumber = dto.ApartmentNumber,
            Password = dto.Password,
            Role = dto.Role,
            PhoneNumber = dto.PhoneNumber
        };

        await _userRepository.AddAsync(user);
        return user.Id;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.Id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {dto.Id} not found");

        user.FullName = dto.FullName ?? user.FullName;
        user.ApartmentNumber = dto.ApartmentNumber ?? user.ApartmentNumber;

        // Хэшируем новый пароль, если он передан
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.Password = HashPassword(dto.Password); // Метод для хэширования
        }

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
    
    /// <summary>
    /// Хэширует пароль с использованием SHA256
    /// </summary>
    /// <param name="password">Открытый пароль</param>
    /// <returns>Хэшированный пароль</returns>
    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}