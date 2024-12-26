using Application.Interfaces.Commands;
using Application.Models;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="UserCommandService"/>
/// </summary>
public class UserCommandServiceTests
{
    /// <summary>
    /// Мок-объект для репозитория пользователей
    /// </summary>
    private readonly Mock<IUserRepository> _userRepositoryMock;

    /// <summary>
    /// Сервис для выполнения команд пользователей
    /// </summary>
    private readonly IUserCommandService _userCommandService;

    /// <summary>
    /// Инициализация тестового класса <see cref="UserCommandServiceTests"/>
    /// </summary>
    public UserCommandServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userCommandService = new UserCommandService(_userRepositoryMock.Object);
    }

    /// <summary>
    /// Тест метода <see cref="IUserCommandService.AddUserAsync"/>, проверяющий успешное добавление пользователя
    /// </summary>
    [Fact]
    public async Task AddUserAsync_Should_Add_User_Successfully()
    {
        var newUser = new AddUserDto
        {
            FullName = "John Doe",
            ApartmentNumber = 101,
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User
        };
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = newUser.FullName,
            ApartmentNumber = newUser.ApartmentNumber,
            Password = newUser.Password,
            PhoneNumber = newUser.PhoneNumber,
            Role = newUser.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _userRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(user);
        
        var result = await _userCommandService.AddUserAsync(newUser);
        
        result.Should().NotBeNull();
        result.FullName.Should().Be(newUser.FullName);
        result.ApartmentNumber.Should().Be(newUser.ApartmentNumber);
        _userRepositoryMock.Verify(repo =>
            repo.AddAsync(It.Is<User>(u =>
                u.FullName == newUser.FullName &&
                u.ApartmentNumber == newUser.ApartmentNumber &&
                u.Password == newUser.Password &&
                u.PhoneNumber == newUser.PhoneNumber &&
                u.Role == newUser.Role)), Times.Once);
    }

    /// <summary>
    /// Тест метода <see cref="IUserCommandService.UpdateUserAsync"/>, проверяющий успешное обновление пользователя
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_Should_Update_User_Successfully()
    {
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FullName = "John Doe",
            ApartmentNumber = 101,
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User
        };

        var updateUserDto = new UpdateUserDto
        {
            Id = userId,
            FullName = "Jane Doe",
            ApartmentNumber = 102
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        await _userCommandService.UpdateUserAsync(updateUserDto);

        existingUser.FullName.Should().Be("Jane Doe");
        existingUser.ApartmentNumber.Should().Be(102);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserCommandService.GetUserByIdAsync"/>, проверяющий успешное получение пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_User_When_Exists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FullName = "John Doe",
            ApartmentNumber = 101,
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var result = await _userCommandService.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.FullName.Should().Be(existingUser.FullName);
        result.ApartmentNumber.Should().Be(existingUser.ApartmentNumber);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserCommandService.GetUserByIdAsync"/>, проверяющий отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        var result = await _userCommandService.GetUserByIdAsync(userId);

        result.Should().BeNull();
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserCommandService.DeleteUserAsync"/>, проверяющий удаление пользователя
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "John Doe",
            ApartmentNumber = 101,
            Password = "securepassword",
            Role = UserRole.User
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        await _userCommandService.DeleteUserAsync(userId);

        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserCommandService.GetUserByIdAsync"/>, проверяющий удаление, когда пользователя не существует
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User?)null); // Возвращаем null, чтобы имитировать отсутствие пользователя

        Func<Task> act = () => _userCommandService.DeleteUserAsync(userId);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage($"User with ID '{userId}' not found.");

        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<User>()), Times.Never);
    }
}