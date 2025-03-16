using Application.Interfaces.Repositories;
using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Orders.Query;

public class MeterReadingQueryTests
{
    private readonly Mock<IMeterReadingRepository> _repositoryMock;
    private readonly Mock<ICachedRepository<MeterReadingEntity, Guid>> _cachedRepositoryMock;
    private readonly Mock<ILogger<MeterReadingQuery>> _loggerMock;
    private readonly MeterReadingQuery _query;

    public MeterReadingQueryTests()
    {
        _repositoryMock = new Mock<IMeterReadingRepository>();
        _cachedRepositoryMock = new Mock<ICachedRepository<MeterReadingEntity, Guid>>();
        _loggerMock = new Mock<ILogger<MeterReadingQuery>>();
        _query = new MeterReadingQuery(
            _repositoryMock.Object, 
            _cachedRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllMeterReadingAsync_ShouldReturnAllReadings()
    {
        // Arrange
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), WaterMeterId = Guid.NewGuid(), WaterValue = "12345", DifferenceValue = 200, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() },
            new() { Id = Guid.NewGuid(), WaterMeterId = Guid.NewGuid(), WaterValue = "67890", DifferenceValue = 4500, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() }
        };
        
        _cachedRepositoryMock.Setup(repo => repo.GetAllCachedAsync(It.IsAny<int>())).ReturnsAsync(expectedReadings);

        // Act
        var result = await _query.GetAllMeterReadingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedReadings.Count, result.Count());
        _cachedRepositoryMock.Verify(repo => repo.GetAllCachedAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetMeterReadingByWaterMeterIdAsync_ShouldReturnReadingsForUser()
    {
        // Arrange
        var waterMeterId = Guid.NewGuid();
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), WaterMeterId = waterMeterId, WaterValue = "12345", DifferenceValue = 100, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() }
        };
        _repositoryMock.Setup(repo => repo.GetByWaterMeterIdAsync(waterMeterId)).ReturnsAsync(expectedReadings);

        // Act
        var result = await _query.GetMeterReadingByWaterMeterIdAsync(waterMeterId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(waterMeterId, result.First().WaterMeterId);
    }

    [Fact]
    public async Task GetMeterReadingByIdAsync_ShouldReturnCorrectReading()
    {
        // Arrange
        var readingId = Guid.NewGuid();
        var expectedReading = new MeterReadingEntity
        {
            Id = readingId, WaterMeterId = Guid.NewGuid(), WaterValue = "12345", DifferenceValue = 100, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity()
        };
        
        // Настраиваем кэш на возврат null, чтобы запрос ушел в репозиторий
        _cachedRepositoryMock.Setup(repo => repo.GetByIdCachedAsync(readingId, It.IsAny<int>())).ReturnsAsync((MeterReadingEntity)null);
        _repositoryMock.Setup(repo => repo.GetByIdAsync(readingId)).ReturnsAsync(expectedReading);

        // Act
        var result = await _query.GetMeterReadingByIdAsync(readingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(readingId, result.Id);
        _cachedRepositoryMock.Verify(repo => repo.GetByIdCachedAsync(readingId, It.IsAny<int>()), Times.Once);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(readingId), Times.Once);
    }
}