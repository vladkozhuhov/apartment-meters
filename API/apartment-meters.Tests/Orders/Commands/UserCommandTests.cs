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
/// Тесты для <see cref="UserCommand"/>
/// </summary>
public class UserCommandTests
{
    /// <summary>
    /// Мок-объект для репозитория пользователей
    /// </summary>
    private readonly Mock<IUserRepository> _userRepositoryMock;

    /// <summary>
    /// Сервис для выполнения команд пользователей
    /// </summary>
    private readonly IUserCommand _userCommand;
    
    /// <summary>
    /// Сервис для выполнения команд пользователей
    /// </summary>
    private readonly IUserQuery _userQuery;

    /// <summary>
    /// Инициализация тестового класса <see cref="UserCommandTests"/>
    /// </summary>
    public UserCommandTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userCommand = new UserCommand(_userRepositoryMock.Object);
    }
    
    // #region Add
    //
    // /// <summary>
    // /// Тест метода <see cref="IUserCommand.AddUserAsync"/>, проверяющий успешное добавление пользователя
    // /// </summary>
    // [Fact]
    // public async Task AddUserAsync_Should_Add_User_Successfully()
    // {
    //     var newUser = new AddUserDto
    //     {
    //         FullName = "John Doe",
    //         ApartmentNumber = 101,
    //         Password = "password123",
    //         PhoneNumber = "1234567890",
    //         Role = UserRole.User
    //     };
    //     
    //     var user = new UserEntity
    //     {
    //         Id = Guid.NewGuid(),
    //         FullName = newUser.FullName,
    //         ApartmentNumber = newUser.ApartmentNumber,
    //         Password = newUser.Password,
    //         PhoneNumber = newUser.PhoneNumber,
    //         Role = newUser.Role,
    //         CreatedAt = DateTime.UtcNow,
    //         UpdatedAt = DateTime.UtcNow
    //     };
    //     
    //     _userRepositoryMock
    //         .Setup(repo => repo.AddAsync(It.IsAny<UserEntity>()))
    //         .ReturnsAsync(user);
    //     
    //     var result = await _userCommand.AddUserAsync(newUser);
    //     
    //     result.Should().NotBeNull();
    //     result.FullName.Should().Be(newUser.FullName);
    //     result.ApartmentNumber.Should().Be(newUser.ApartmentNumber);
    //     _userRepositoryMock.Verify(repo =>
    //         repo.AddAsync(It.Is<UserEntity>(u =>
    //             u.FullName == newUser.FullName &&
    //             u.ApartmentNumber == newUser.ApartmentNumber &&
    //             u.Password == newUser.Password &&
    //             u.PhoneNumber == newUser.PhoneNumber &&
    //             u.Role == newUser.Role)), Times.Once);
    // }
    //
    // #endregion

    #region Update

    /// <summary>
    /// Тест метода <see cref="IUserCommand.UpdateUserAsync"/>, проверяющий успешное обновление пользователя
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_Should_Update_User_Successfully()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity
        {
            Id = userId,
            LastName = "Баранов",
            FirstName = "Михаил",
            MiddleName = "Петрович",
            ApartmentNumber = 101,
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User
        };

        var updateUserDto = new UpdateUserDto
        {
            Id = userId,
            LastName = "Баранов",
            FirstName = "Валерий",
            MiddleName = "Петрович",
            ApartmentNumber = 102
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<UserEntity>()))
            .Returns(Task.CompletedTask);

        await _userCommand.UpdateUserAsync(userId, updateUserDto);

        existingUser.FirstName.Should().Be("Jane Doe");
        existingUser.ApartmentNumber.Should().Be(102);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
    }

    #endregion

    #region Get

    /// <summary>
    /// Тест метода <see cref="IUserQuery.GetUserByIdAsync"/>, проверяющий успешное получение пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_User_When_Exists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity
        {
            Id = userId,
            LastName = "Баранов",
            FirstName = "Валерий",
            MiddleName = "Петрович",
            ApartmentNumber = 101,
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var result = await _userQuery.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        // result.FullName.Should().Be(existingUser.FullName);
        result.ApartmentNumber.Should().Be(existingUser.ApartmentNumber);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserQuery.GetUserByIdAsync"/>, проверяющий отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((UserEntity)null);

        var result = await _userQuery.GetUserByIdAsync(userId);

        result.Should().BeNull();
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IUserQuery.GetAllUsersAsync"/>, проверяющий успешное получение всех пользователей
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        var users = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), ApartmentNumber = 1, LastName = "Баранов1", FirstName = "Валерий1", MiddleName = "Петрович1", Password = "pass1", Role = UserRole.User },
            new UserEntity { Id = Guid.NewGuid(), ApartmentNumber = 2, LastName = "Баранов2", FirstName = "Валерий2", MiddleName = "Петрович2", Password = "pass2", Role = UserRole.Admin }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(users);

        var result = await _userQuery.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);

        _userRepositoryMock
            .Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    #endregion

    #region Delete

    /// <summary>
    /// Тест метода <see cref="IUserCommand.DeleteUserAsync"/>, проверяющий удаление пользователя
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new UserEntity
        {
            Id = userId,
            LastName = "Баранов",
            FirstName = "Валерий",
            MiddleName = "Петрович",
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

        await _userCommand.DeleteUserAsync(userId);

        _userRepositoryMock
            .Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock
            .Verify(repo => repo.DeleteAsync(It.Is<UserEntity>(u => u.Id == userId)), Times.Once);
    }

    #endregion
}