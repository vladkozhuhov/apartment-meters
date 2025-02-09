using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Query;

/// <summary>
/// Тесты для <see cref="UserQuery"/>
/// </summary>
public class UserQueryTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserQuery _userQuery;
    
    public UserQueryTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userQuery = new UserQuery(_userRepositoryMock.Object);
    }
    
    /// <summary>
    /// Проверяет успешное получение пользователя по его идентификатору
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var expectedUser = new UserEntity { Id = userId };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);
        var result = await _userQuery.GetUserByIdAsync(userId);
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }
    
    /// <summary>
    /// Проверяет получение пользователя с неверным идентификатором
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserEntity)null);
        var result = await _userQuery.GetUserByIdAsync(userId);
        Assert.Null(result);
    }
    
    /// <summary>
    /// Проверяет успешное получение списка всех пользователей
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsers()
    {
        var users = new List<UserEntity>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };
        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(users);
        var result = await _userQuery.GetAllUsersAsync();
        Assert.Equal(2, result.Count());
    }
}