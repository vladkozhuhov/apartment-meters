using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Query;

public class MeterReadingQueryTests
{
    private readonly Mock<IMeterReadingRepository> _repositoryMock;
    private readonly MeterReadingQuery _query;

    public MeterReadingQueryTests()
    {
        _repositoryMock = new Mock<IMeterReadingRepository>();
        _query = new MeterReadingQuery(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllMeterReadingAsync_ShouldReturnAllReadings()
    {
        // Arrange
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), WaterMeterId = Guid.NewGuid(), WaterValue = "12345", TotalValue = 66666, DifferenceValue = 200, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() },
            new() { Id = Guid.NewGuid(), WaterMeterId = Guid.NewGuid(), WaterValue = "67890", TotalValue = 77777, DifferenceValue = 4500, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() }
        };
        _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedReadings);

        // Act
        var result = await _query.GetAllMeterReadingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedReadings.Count, result.Count());
    }

    [Fact]
    public async Task GetMeterReadingByWaterMeterIdAsync_ShouldReturnReadingsForUser()
    {
        // Arrange
        var waterMeterId = Guid.NewGuid();
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), WaterMeterId = waterMeterId, WaterValue = "12345", TotalValue = 66666, DifferenceValue = 100, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity() }
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
            Id = readingId, WaterMeterId = Guid.NewGuid(), WaterValue = "12345", TotalValue = 66666, DifferenceValue = 100, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, WaterMeter = new WaterMeterEntity()
        };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(readingId)).ReturnsAsync(expectedReading);

        // Act
        var result = await _query.GetMeterReadingByIdAsync(readingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(readingId, result.Id);
    }
}