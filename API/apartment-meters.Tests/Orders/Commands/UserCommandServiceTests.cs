using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
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
    /// Сервис для выполнения команд пользователей
    /// </summary>
    private readonly IUserQueryService _userQueryService;

    /// <summary>
    /// Инициализация тестового класса <see cref="UserCommandServiceTests"/>
    /// </summary>
    public UserCommandServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userCommandService = new UserCommandService(_userRepositoryMock.Object);
    }
    
    #region Add

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
        
        var user = new UserEntity
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
            .Setup(repo => repo.AddAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(user);
        
        var result = await _userCommandService.AddUserAsync(newUser);
        
        result.Should().NotBeNull();
        result.FullName.Should().Be(newUser.FullName);
        result.ApartmentNumber.Should().Be(newUser.ApartmentNumber);
        _userRepositoryMock.Verify(repo =>
            repo.AddAsync(It.Is<UserEntity>(u =>
                u.FullName == newUser.FullName &&
                u.ApartmentNumber == newUser.ApartmentNumber &&
                u.Password == newUser.Password &&
                u.PhoneNumber == newUser.PhoneNumber &&
                u.Role == newUser.Role)), Times.Once);
    }
    
    #endregion

    #region Update

    /// <summary>
    /// Тест метода <see cref="IUserCommandService.UpdateUserAsync"/>, проверяющий успешное обновление пользователя
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_Should_Update_User_Successfully()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity
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
            .Setup(repo => repo.UpdateAsync(It.IsAny<UserEntity>()))
            .Returns(Task.CompletedTask);

        await _userCommandService.UpdateUserAsync(userId, updateUserDto);

        existingUser.FullName.Should().Be("Jane Doe");
        existingUser.ApartmentNumber.Should().Be(102);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
    }

    #endregion

    #region Get

    /// <summary>
    /// Тест метода <see cref="IUserQueryService.GetUserByIdAsync"/>, проверяющий успешное получение пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_User_When_Exists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity
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

        var result = await _userQueryService.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.FullName.Should().Be(existingUser.FullName);
        result.ApartmentNumber.Should().Be(existingUser.ApartmentNumber);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserQueryService.GetUserByIdAsync"/>, проверяющий отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((UserEntity)null);

        var result = await _userQueryService.GetUserByIdAsync(userId);

        result.Should().BeNull();
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserQueryService.GetAllUsersAsync"/>, проверяющий успешное получение всех пользователей
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        var users = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), ApartmentNumber = 1, FullName = "User 1", Password = "pass1", Role = UserRole.User },
            new UserEntity { Id = Guid.NewGuid(), ApartmentNumber = 2, FullName = "User 2", Password = "pass2", Role = UserRole.Admin }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(users);

        var result = await _userQueryService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);

        _userRepositoryMock
            .Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    #endregion

    #region Delete

    /// <summary>
    /// Тест метода <see cref="IUserCommandService.DeleteUserAsync"/>, проверяющий удаление пользователя
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new UserEntity
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
            .Setup(repo => repo.DeleteAsync(It.IsAny<UserEntity>()))
            .Returns(Task.CompletedTask);

        await _userCommandService.DeleteUserAsync(userId);

        _userRepositoryMock
            .Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock
            .Verify(repo => repo.DeleteAsync(It.Is<UserEntity>(u => u.Id == userId)), Times.Once);
    }

    #endregion
}