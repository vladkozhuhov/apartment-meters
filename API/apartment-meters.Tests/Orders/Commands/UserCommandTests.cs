using Application.Models.UsersModel;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="UserCommand"/>
/// </summary>
public class UserCommandTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserCommand _userCommand;

    /// <summary>
    /// Инициализирует тестовый контекст, создавая моки и сервис команд пользователей.
    /// </summary>
    public UserCommandTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userCommand = new UserCommand(_userRepositoryMock.Object);
    }

    /// <summary>
    /// Проверяет успешное добавление нового пользователя в систему.
    /// </summary>
    [Fact]
    public async Task AddUserAsync_ShouldAddUser()
    {
        var dto = new AddUserDto
        {
            ApartmentNumber = 12,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "password123",
            PhoneNumber = "1234567890",
            Role = UserRole.User,
            FactoryNumber = "12345",
            FactoryYear = DateTime.UtcNow
        };

        var result = await _userCommand.AddUserAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.ApartmentNumber, result.ApartmentNumber);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    /// <summary>
    /// Проверяет успешное обновление данных существующего пользователя.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateExistingUser()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity
        {
            Id = userId,
            LastName = "Петров",
            FirstName = "Петр",
            ApartmentNumber = 15,
            FactoryNumber = "54321",
            FactoryYear = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var dto = new UpdateUserDto
        {
            LastName = "Сидоров",
            FirstName = "Сидор"
        };

        await _userCommand.UpdateUserAsync(userId, dto);

        Assert.Equal(dto.LastName, existingUser.LastName);
        Assert.Equal(dto.FirstName, existingUser.FirstName);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
    }

    /// <summary>
    /// Проверяет попытку обновления несуществующего пользователя.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserEntity)null);

        var dto = new UpdateUserDto { LastName = "Тест" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userCommand.UpdateUserAsync(userId, dto));
    }

    /// <summary>
    /// Проверяет удаление пользователя из системы.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser()
    {
        var userId = Guid.NewGuid();
        var existingUser = new UserEntity { Id = userId };

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        await _userCommand.DeleteUserAsync(userId);

        _userRepositoryMock.Verify(repo => repo.DeleteAsync(existingUser), Times.Once);
    }

    /// <summary>
    /// Проверяет попытку удаления несуществующего пользователя.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userCommand.DeleteUserAsync(userId));
    }
}