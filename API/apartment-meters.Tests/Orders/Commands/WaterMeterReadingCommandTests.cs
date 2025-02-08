using Application.Models.MeterReading;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="WaterMeterReadingCommand"/>
/// </summary>
public class WaterMeterReadingCommandTests
{
    private readonly Mock<IWaterMeterReadingRepository> _repositoryMock;
    private readonly WaterMeterReadingCommand _command;
    
    public WaterMeterReadingCommandTests()
    {
        _repositoryMock = new Mock<IWaterMeterReadingRepository>();
        _command = new WaterMeterReadingCommand(_repositoryMock.Object);
    }

    /// <summary>
    /// Проверяет успешное добавление нового показания водомера
    /// </summary>
    [Fact]
    public async Task AddMeterReadingAsync_ShouldAddReading()
    {
        var dto = new AddWaterMeterReadingDto
        {
            UserId = Guid.NewGuid(),
            PrimaryColdWaterValue = "12345",
            PrimaryHotWaterValue = "54321",
            HasSecondaryMeter = true,
            SecondaryColdWaterValue = "11111",
            SecondaryHotWaterValue = "22222",
            ReadingDate = DateTime.UtcNow
        };

        var result = await _command.AddMeterReadingAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.UserId, result.UserId);
        Assert.Equal(dto.PrimaryColdWaterValue, result.PrimaryColdWaterValue);
        Assert.Equal(dto.PrimaryHotWaterValue, result.PrimaryHotWaterValue);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<MeterReadingEntity>()), Times.Once);
    }

    /// <summary>
    /// Проверяет успешное обновление данных существующего пользователя
    /// </summary>
    [Fact]
    public async Task UpdateMeterReadingAsync_ShouldUpdateReading()
    {
        var id = Guid.NewGuid();
        var existingReading = new MeterReadingEntity { Id = id, UserId = Guid.NewGuid() };
        var dto = new UpdateWaterMeterReadingDto
        {
            UserId = existingReading.UserId,
            PrimaryColdWaterValue = "67890",
            PrimaryHotWaterValue = "09876",
            HasSecondaryMeter = false
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingReading);

        await _command.UpdateMeterReadingAsync(id, dto);

        Assert.Equal(dto.PrimaryColdWaterValue, existingReading.PrimaryColdWaterValue);
        Assert.Equal(dto.PrimaryHotWaterValue, existingReading.PrimaryHotWaterValue);
        _repositoryMock.Verify(r => r.UpdateAsync(existingReading), Times.Once);
    }

    /// <summary>
    /// Проверяет успешное обновление данных не существующего пользователя
    /// </summary>
    [Fact]
    public void UpdateMeterReadingAsync_ShouldThrowIfNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateWaterMeterReadingDto { UserId = Guid.NewGuid() };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((MeterReadingEntity)null);

        Assert.ThrowsAsync<KeyNotFoundException>(() => _command.UpdateMeterReadingAsync(id, dto));
    }

    /// <summary>
    /// Проверяет удаление показания водомера из системы
    /// </summary>
    [Fact]
    public async Task DeleteMeterReadingAsync_ShouldDeleteReading()
    {
        var id = Guid.NewGuid();
        var existingReading = new MeterReadingEntity { Id = id };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingReading);

        await _command.DeleteMeterReadingAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(existingReading), Times.Once);
    }

    /// <summary>
    /// Проверяет попытку удаления несуществующего показания водомера
    /// </summary>
    [Fact]
    public void DeleteMeterReadingAsync_ShouldThrowIfNotFound()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((MeterReadingEntity)null);

        Assert.ThrowsAsync<KeyNotFoundException>(() => _command.DeleteMeterReadingAsync(id));
    }
}