using Application.Models.WaterMeterModel;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Orders.Commands;

public class WaterMeterCommandTests
{
    private readonly Mock<IWaterMeterRepository> _repositoryMock;
    private readonly WaterMeterCommand _command;

    public WaterMeterCommandTests()
    {
        _repositoryMock = new Mock<IWaterMeterRepository>();
        _command = new WaterMeterCommand(_repositoryMock.Object);
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
}