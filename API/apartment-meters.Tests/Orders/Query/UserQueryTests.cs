using Application.Interfaces.Repositories;
using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Orders.Query;

/// <summary>
/// Тесты для <see cref="UserQuery"/>
/// </summary>
public class UserQueryTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICachedRepository<UserEntity, Guid>> _cachedRepositoryMock;
    private readonly Mock<ILogger<UserQuery>> _loggerMock;
    private readonly Mock<IWaterMeterRepository> _waterMeterRepositoryMock;
    private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;
    private readonly UserQuery _userQuery;
    
    /// <summary>
    /// Инициализирует тестовый контекст, создавая моки и сервис запросов пользователей.
    /// </summary>
    public UserQueryTests()
    {
        _waterMeterRepositoryMock = new Mock<IWaterMeterRepository>();
        _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _cachedRepositoryMock = new Mock<ICachedRepository<UserEntity, Guid>>();
        _loggerMock = new Mock<ILogger<UserQuery>>();
        _userQuery = new UserQuery(
            _userRepositoryMock.Object, 
            _cachedRepositoryMock.Object,
            _waterMeterRepositoryMock.Object,
            _meterReadingRepositoryMock.Object,
            _loggerMock.Object);
    }
    
    /// <summary>
    /// Проверяет успешное получение пользователя по его идентификатору
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var expectedUser = new UserEntity { Id = userId };
        
        // Настраиваем кэш на возврат null, чтобы запрос ушел в репозиторий
        _cachedRepositoryMock.Setup(repo => repo.GetByIdCachedAsync(userId, It.IsAny<int>())).ReturnsAsync((UserEntity)null);
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);
            
        var result = await _userQuery.GetUserByIdAsync(userId);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        _cachedRepositoryMock.Verify(repo => repo.GetByIdCachedAsync(userId, It.IsAny<int>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Проверяет получение пользователя с неверным идентификатором
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserEntity)null);
        _cachedRepositoryMock.Setup(repo => repo.GetByIdCachedAsync(userId, It.IsAny<int>())).ReturnsAsync((UserEntity)null);
        
        // Теперь ожидаем исключение вместо null
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userQuery.GetUserByIdAsync(userId));
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
        
        _cachedRepositoryMock.Setup(repo => repo.GetAllCachedAsync(It.IsAny<int>()))
            .ReturnsAsync(users);
            
        var result = await _userQuery.GetAllUsersAsync();
        
        Assert.Equal(2, result.Count());
        _cachedRepositoryMock.Verify(repo => repo.GetAllCachedAsync(It.IsAny<int>()), Times.Once);
    }
}