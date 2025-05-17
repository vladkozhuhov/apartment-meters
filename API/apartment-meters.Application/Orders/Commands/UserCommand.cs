using Application.Interfaces.Commands;
using Application.Models;
using Application.Models.UsersModel;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace Application.Orders.Commands;

/// <summary>
/// Сервис для выполнения команд, связанных с пользователями
/// </summary>
public class UserCommand : IUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserCommand> _logger;

    /// <summary>
    /// Конструктор сервиса команд для пользователей
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей</param>
    /// <param name="logger">Сервис логирования</param>
    public UserCommand(
        IUserRepository userRepository,
        ILogger<UserCommand> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<UserEntity> AddUserAsync(UserAddDto dto)
    {
        _logger.LogInformation("Добавление нового пользователя");
        
        // Хешируем пароль перед сохранением
        string hashedPassword = HashPassword(dto.Password);
        
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            ApartmentNumber = dto.ApartmentNumber,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            Password = hashedPassword, // Используем хешированный пароль
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("Пользователь с ID {UserId} успешно добавлен", user.Id);
        
        return user;
    }
    
    /// <inheritdoc />
    public async Task UpdateUserAsync(Guid id, UserUpdateDto dto)
    {
        _logger.LogInformation("Обновление пользователя с ID {UserId}", id);
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Пользователь с ID {UserId} не найден", id);
            throw new KeyNotFoundException($"Пользователь с ID {id} не найден");
        }

        user.LastName = dto.LastName ?? user.LastName;
        user.FirstName = dto.FirstName ?? user.FirstName;
        user.MiddleName = dto.MiddleName ?? user.MiddleName;
        user.ApartmentNumber = dto.ApartmentNumber ?? user.ApartmentNumber;
        
        // Обновляем пароль только если он был предоставлен и не пустой
        if (!string.IsNullOrEmpty(dto.Password))
        {
            // Если пароль начинается с $ - это может быть хешированный пароль
            if (dto.Password.StartsWith("$2a$") || dto.Password.StartsWith("$2b$") || dto.Password.StartsWith("$2y$"))
            {
                // Пароль уже хеширован (например, из админ-панели)
                user.Password = dto.Password;
            }
            else
            {
                // Хешируем новый пароль
                user.Password = HashPassword(dto.Password);
            }
            
            _logger.LogInformation("Пароль пользователя с ID {UserId} был обновлен", id);
        }
        else 
        {
            _logger.LogInformation("Пароль пользователя с ID {UserId} остался без изменений (пустой пароль в запросе)", id);
        }
        
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
        user.Role = dto.Role ?? user.Role;

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Пользователь с ID {UserId} успешно обновлен", id);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Удаление пользователя с ID {UserId}", id);
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Пользователь с ID {UserId} не найден", id);
            throw new KeyNotFoundException($"Пользователь с ID {id} не найден");
        }

        await _userRepository.DeleteAsync(user);
        _logger.LogInformation("Пользователь с ID {UserId} успешно удален", id);
    }
    
    /// <inheritdoc />
    public async Task UpdateUserPhoneAsync(Guid userId, PhoneUpdateDto phoneDto)
    {
        _logger.LogInformation("Обновление номера телефона для пользователя с ID {UserId}", userId);
        _logger.LogInformation("Полученный DTO: {@PhoneDto}", phoneDto);
        
        if (phoneDto == null)
        {
            _logger.LogError("PhoneUpdateDto равен null");
            throw new ArgumentNullException(nameof(phoneDto));
        }
        
        if (string.IsNullOrEmpty(phoneDto.PhoneNumber))
        {
            _logger.LogError("PhoneNumber в DTO равен null или пустой строке");
            throw new ArgumentException("Номер телефона не может быть пустым", nameof(phoneDto));
        }
        
        try
        {
            // Используем прямой SQL-запрос для обновления телефона без загрузки всей сущности
            // и избегаем проблемы с отслеживанием сущностей
            await _userRepository.UpdatePhoneAsync(userId, phoneDto.PhoneNumber);
            _logger.LogInformation("Номер телефона пользователя с ID {UserId} успешно обновлен на {NewPhone}", 
                userId, phoneDto.PhoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении номера телефона пользователя {UserId}", userId);
            throw;
        }
    }
    
    /// <summary>
    /// Хеширует пароль с использованием BCrypt
    /// </summary>
    /// <param name="password">Пароль в открытом виде</param>
    /// <returns>Хешированный пароль</returns>
    private string HashPassword(string password)
    {
        try
        {
            // Генерируем соль и хешируем пароль
            // WorkFactor 12 - оптимальный баланс между безопасностью и производительностью
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при хешировании пароля");
            throw;
        }
    }
}