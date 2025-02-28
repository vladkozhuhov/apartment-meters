using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Query;

public class WaterMeterQueryTests
{
    private readonly Mock<IWaterMeterRepository> _repositoryMock;
    private readonly WaterMeterQuery _query;

    public WaterMeterQueryTests()
    {
        _repositoryMock = new Mock<IWaterMeterRepository>();
        _query = new WaterMeterQuery(_repositoryMock.Object);
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
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM123456", FactoryYear = DateTime.UtcNow, UserId = userId },
            new() { Id = Guid.NewGuid(), FactoryNumber = "WM654321", FactoryYear = DateTime.UtcNow, UserId = userId }
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
            Id = meterId, FactoryNumber = "WM123456", FactoryYear = DateTime.UtcNow
        };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(meterId)).ReturnsAsync(expectedMeter);

        // Act
        var result = await _query.GetWaterMeterByIdAsync(meterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meterId, result.Id);
    }
}