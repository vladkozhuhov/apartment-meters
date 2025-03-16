using Application.Interfaces.Repositories;
using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Orders.Query;

public class WaterMeterQueryTests
{
    private readonly Mock<IWaterMeterRepository> _repositoryMock;
    private readonly Mock<ICachedRepository<WaterMeterEntity, Guid>> _cachedRepositoryMock;
    private readonly Mock<ILogger<WaterMeterQuery>> _loggerMock;
    private readonly WaterMeterQuery _query;

    public WaterMeterQueryTests()
    {
        _repositoryMock = new Mock<IWaterMeterRepository>();
        _cachedRepositoryMock = new Mock<ICachedRepository<WaterMeterEntity, Guid>>();
        _loggerMock = new Mock<ILogger<WaterMeterQuery>>();
        _query = new WaterMeterQuery(
            _repositoryMock.Object, 
            _cachedRepositoryMock.Object,
            _loggerMock.Object);
    }

    /// <summary>
    /// Проверяет, что метод возвращает счетчики по идентификатору пользователя.
    /// </summary>
    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnWaterMetersForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedMeters = new List<WaterMeterEntity>
        {
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM123456", FactoryYear = DateOnly.Parse("2025-01-01"), UserId = userId },
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM654321", FactoryYear = DateOnly.Parse("2025-01-01"), UserId = userId }
        };
        _repositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(expectedMeters);

        // Act
        var result = await _query.GetWaterMeterByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMeters.Count, result.Count());
        Assert.All(result, meter => Assert.Equal(userId, meter.UserId));
    }

    /// <summary>
    /// Проверяет, что метод возвращает счетчик по идентификатору.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectWaterMeter()
    {
        // Arrange
        var meterId = Guid.NewGuid();
        var expectedMeter = new WaterMeterEntity
        {
            Id = meterId, FactoryNumber = "WM123456", FactoryYear = DateOnly.Parse("2025-01-01")
        };
        
        // Настраиваем кэш на возврат null, чтобы запрос ушел в репозиторий
        _cachedRepositoryMock.Setup(repo => repo.GetByIdCachedAsync(meterId, It.IsAny<int>())).ReturnsAsync((WaterMeterEntity)null);
        _repositoryMock.Setup(repo => repo.GetByIdAsync(meterId)).ReturnsAsync(expectedMeter);

        // Act
        var result = await _query.GetWaterMeterByIdAsync(meterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meterId, result.Id);
        _cachedRepositoryMock.Verify(repo => repo.GetByIdCachedAsync(meterId, It.IsAny<int>()), Times.Once);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(meterId), Times.Once);
    }

    /// <summary>
    /// Проверяет, что метод возвращает все счетчики воды
    /// </summary>
    [Fact]
    public async Task GetAllWaterMetersAsync_ShouldReturnAllWaterMeters()
    {
        // Arrange
        var expectedMeters = new List<WaterMeterEntity>
        {
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM123456", FactoryYear = DateOnly.Parse("2025-01-01") },
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM654321", FactoryYear = DateOnly.Parse("2025-01-01") }
        };
        
        _cachedRepositoryMock.Setup(repo => repo.GetAllCachedAsync(It.IsAny<int>())).ReturnsAsync(expectedMeters);

        // Act
        var result = await _query.GetAllWaterMetersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMeters.Count, result.Count());
        _cachedRepositoryMock.Verify(repo => repo.GetAllCachedAsync(It.IsAny<int>()), Times.Once);
    }
}