using Application.Models.MeterReadingModel;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="MeterReadingCommand"/>
/// </summary>
public class MeterReadingCommandTests
{
    private readonly Mock<IMeterReadingRepository> _repositoryMock;
    private readonly Mock<ILogger<MeterReadingCommand>> _loggerMock;
    private readonly MeterReadingCommand _command;
    
    public MeterReadingCommandTests()
    {
        _repositoryMock = new Mock<IMeterReadingRepository>();
        _loggerMock = new Mock<ILogger<MeterReadingCommand>>();
        _command = new MeterReadingCommand(_repositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Проверяет успешное добавление нового показания водомера
    /// </summary>
    [Fact]
    public async Task AddMeterReadingAsync_ShouldAddReading()
    {
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123,45",
            ReadingDate = DateTime.UtcNow
        };

        var result = await _command.AddMeterReadingAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.WaterMeterId, result.WaterMeterId);
        Assert.NotEqual(dto.WaterValue, result.WaterValue);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<MeterReadingEntity>()), Times.Once);
    }

    /// <summary>
    /// Проверяет успешное обновление данных существующего пользователя
    /// </summary>
    [Fact]
    public async Task UpdateMeterReadingAsync_ShouldUpdateReading()
    {
        var id = Guid.NewGuid();
        var existingReading = new MeterReadingEntity { Id = id, WaterMeterId = Guid.NewGuid() };
        var dto = new MeterReadingUpdateDto
        {
            WaterMeterId = existingReading.WaterMeterId,
            WaterValue = "678,90",
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingReading);

        await _command.UpdateMeterReadingAsync(id, dto);

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
        var dto = new MeterReadingUpdateDto { WaterMeterId = Guid.NewGuid() };

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