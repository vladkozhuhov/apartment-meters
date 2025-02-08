using Application.Orders.Queries;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Query;

public class WaterMeterReadingQueryTests
{
    private readonly Mock<IWaterMeterReadingRepository> _repositoryMock;
    private readonly WaterMeterReadingQuery _query;

    public WaterMeterReadingQueryTests()
    {
        _repositoryMock = new Mock<IWaterMeterReadingRepository>();
        _query = new WaterMeterReadingQuery(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllMeterReadingAsync_ShouldReturnAllReadings()
    {
        // Arrange
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), PrimaryColdWaterValue = "12345", PrimaryHotWaterValue = "54321", PrimaryTotalValue = 66666, PrimaryDifferenceValue = 100, HasSecondaryMeter = true, SecondaryColdWaterValue = "11111", SecondaryHotWaterValue = "22222", SecondaryTotalValue = 33333, SecondaryDifferenceValue = 50, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UserEntity = new UserEntity() },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), PrimaryColdWaterValue = "67890", PrimaryHotWaterValue = "09876", PrimaryTotalValue = 77777, PrimaryDifferenceValue = 200, HasSecondaryMeter = false, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UserEntity = new UserEntity() }
        };
        _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedReadings);

        // Act
        var result = await _query.GetAllMeterReadingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedReadings.Count, result.Count());
    }

    [Fact]
    public async Task GetMeterReadingByUserIdAsync_ShouldReturnReadingsForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedReadings = new List<MeterReadingEntity>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, PrimaryColdWaterValue = "12345", PrimaryHotWaterValue = "54321", PrimaryTotalValue = 66666, PrimaryDifferenceValue = 100, HasSecondaryMeter = false, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UserEntity = new UserEntity() }
        };
        _repositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(expectedReadings);

        // Act
        var result = await _query.GetMeterReadingByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(userId, result.First().UserId);
    }

    [Fact]
    public async Task GetMeterReadingByIdAsync_ShouldReturnCorrectReading()
    {
        // Arrange
        var readingId = Guid.NewGuid();
        var expectedReading = new MeterReadingEntity
        {
            Id = readingId, UserId = Guid.NewGuid(), PrimaryColdWaterValue = "12345", PrimaryHotWaterValue = "54321", PrimaryTotalValue = 66666, PrimaryDifferenceValue = 100, HasSecondaryMeter = false, ReadingDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UserEntity = new UserEntity()
        };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(readingId)).ReturnsAsync(expectedReading);

        // Act
        var result = await _query.GetMeterReadingByIdAsync(readingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(readingId, result.Id);
    }
}