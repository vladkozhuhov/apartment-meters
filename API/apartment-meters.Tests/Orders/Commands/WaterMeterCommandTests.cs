using Application.Models.WaterMeterModel;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Orders.Commands;

public class WaterMeterCommandTests
{
    private readonly Mock<IWaterMeterRepository> _repositoryMock;
    private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;
    private readonly Mock<ILogger<WaterMeterCommand>> _loggerMock;
    private readonly WaterMeterCommand _command;

    public WaterMeterCommandTests()
    {
        _repositoryMock = new Mock<IWaterMeterRepository>();
        _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();
        _loggerMock = new Mock<ILogger<WaterMeterCommand>>();
        _command = new WaterMeterCommand(_repositoryMock.Object, _meterReadingRepositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Проверяет успешное добавление нового водомера.
    /// </summary>
    [Fact]
    public async Task AddWaterMeterAsync_ShouldAddWaterMeter()
    {
        // Arrange
        var dto = new WaterMeterAddDto
        {
            FactoryNumber = "WM123456",
            FactoryYear = DateOnly.Parse("2025-01-01")
        };

        // Act
        var result = await _command.AddWaterMeterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.FactoryNumber, result.FactoryNumber);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<WaterMeterEntity>()), Times.Once);
    }

    /// <summary>
    /// Проверяет успешное обновление существующего водомера.
    /// </summary>
    [Fact]
    public async Task UpdateWaterMeterAsync_ShouldUpdateWaterMeter()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingMeter = new WaterMeterEntity { Id = id, FactoryNumber = "WM123456" };
        var dto = new WaterMeterUpdateDto
        {
            FactoryNumber = "WM654321"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMeter);

        // Act
        await _command.UpdateWaterMeterAsync(id, dto);

        // Assert
        Assert.Equal(dto.FactoryNumber, existingMeter.FactoryNumber);
        _repositoryMock.Verify(r => r.UpdateAsync(existingMeter), Times.Once);
    }

    /// <summary>
    /// Проверяет, что при попытке обновления несуществующего водомера выбрасывается исключение.
    /// </summary>
    [Fact]
    public void UpdateWaterMeterAsync_ShouldThrowIfNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new WaterMeterUpdateDto { FactoryNumber = "WM654321" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((WaterMeterEntity)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _command.UpdateWaterMeterAsync(id, dto));
    }

    /// <summary>
    /// Проверяет успешное удаление водомера.
    /// </summary>
    [Fact]
    public async Task DeleteWaterMeterAsync_ShouldDeleteWaterMeter()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingMeter = new WaterMeterEntity { Id = id };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMeter);

        // Act
        await _command.DeleteWaterMeterAsync(id);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(existingMeter), Times.Once);
    }

    /// <summary>
    /// Проверяет, что при попытке удаления несуществующего водомера выбрасывается исключение.
    /// </summary>
    [Fact]
    public void DeleteWaterMeterAsync_ShouldThrowIfNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((WaterMeterEntity)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _command.DeleteWaterMeterAsync(id));
    }

    /// <summary>
    /// Проверяет, что при изменении критических полей (номер или дата) счетчика удаляются все связанные показания.
    /// </summary>
    [Fact]
    public async Task UpdateWaterMeterAsync_ShouldDeleteRelatedReadingsWhenCriticalFieldsChange()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingMeter = new WaterMeterEntity 
        { 
            Id = id, 
            FactoryNumber = "WM123456",
            FactoryYear = new DateOnly(2022, 1, 1)
        };
        
        var dto = new WaterMeterUpdateDto
        {
            FactoryNumber = "WM654321" // Измененный номер счетчика
        };

        var readings = new List<MeterReadingEntity>
        {
            new MeterReadingEntity { Id = Guid.NewGuid(), WaterMeterId = id },
            new MeterReadingEntity { Id = Guid.NewGuid(), WaterMeterId = id }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMeter);
        _meterReadingRepositoryMock.Setup(r => r.GetByWaterMeterIdAsync(id)).ReturnsAsync(readings);

        // Act
        await _command.UpdateWaterMeterAsync(id, dto);

        // Assert
        Assert.Equal(dto.FactoryNumber, existingMeter.FactoryNumber);
        _repositoryMock.Verify(r => r.UpdateAsync(existingMeter), Times.Once);
        
        // Проверяем, что для каждого показания был вызван метод DeleteAsync
        foreach (var reading in readings)
        {
            _meterReadingRepositoryMock.Verify(r => r.DeleteAsync(reading), Times.Once);
        }
    }
}