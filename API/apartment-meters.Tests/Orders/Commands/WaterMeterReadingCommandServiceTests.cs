using Application.Interfaces.Commands;
using Application.Models;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="WaterMeterReadingCommandServiceTests"/>
/// </summary>
public class WaterMeterReadingCommandServiceTests
{
    /// <summary>
    /// Мок-объект для репозитория водомеров
    /// </summary>
    private readonly Mock<IWaterMeterReadingRepository> _waterMeterReadingRepositoryMock;

    /// <summary>
    /// Сервис для выполнения команд водомеров
    /// </summary>
    private readonly IWaterMeterReadingCommandService _waterMeterReadingCommandService;

    /// <summary>
    /// Инициализация тестового класса <see cref="WaterMeterReadingCommandServiceTests"/>
    /// </summary>
    public WaterMeterReadingCommandServiceTests()
    {
        _waterMeterReadingRepositoryMock = new Mock<IWaterMeterReadingRepository>();
        _waterMeterReadingCommandService = new WaterMeterReadingCommandService(_waterMeterReadingRepositoryMock.Object);
    }

    #region Add

    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.AddMeterReadingAsync"/>, проверяющий успешное добавление показателей водомеров
    /// </summary>
    [Fact]
    public async Task AddMeterReadingAsync_Should_Add_WaterMeterReading_Successfully()
    {
        var newWaterMeterReading = new AddWaterMeterReadingDto()
        {
            ColdWaterValue = 34,
            HotWaterValue = 25,
            ReadingDate = DateTime.UtcNow
        };
        
        var waterMeterReading = new MeterReading()
        {
            Id = Guid.NewGuid(),
            ColdWaterValue = newWaterMeterReading.ColdWaterValue,
            HotWaterValue = newWaterMeterReading.HotWaterValue,
            ReadingDate = newWaterMeterReading.ReadingDate
        };
        
        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<MeterReading>()))
            .ReturnsAsync(waterMeterReading);
        
        var result = await _waterMeterReadingCommandService.AddMeterReadingAsync(newWaterMeterReading);
        
        result.Should().NotBeNull();
        result.ColdWaterValue.Should().Be(newWaterMeterReading.ColdWaterValue);
        result.HotWaterValue.Should().Be(newWaterMeterReading.HotWaterValue);
        _waterMeterReadingRepositoryMock.Verify(repo =>
            repo.AddAsync(It.Is<MeterReading>(u =>
                u.ColdWaterValue == newWaterMeterReading.ColdWaterValue &&
                u.ColdWaterValue == newWaterMeterReading.ColdWaterValue &&
                u.HotWaterValue == newWaterMeterReading.HotWaterValue)), Times.Once);
    }

    #endregion
    
    #region Get

    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.UpdateMeterReadingAsync"/>, проверяющий успешное обновление показателей водомеров
    /// </summary>
    [Fact]
    public async Task UpdateWaterMeterReadingAsync_Should_Update_WaterMeterReading_Successfully()
    {
        var userId = Guid.NewGuid();
        var existingWaterMeterReading = new MeterReading()
        {
            Id = userId,
            ColdWaterValue = 34,
            HotWaterValue = 25,
            ReadingDate = DateTime.UtcNow
        };

        var updateWaterMeterReadingDto = new UpdateWaterMeterReadingDto()
        {
            UserId = userId,
            ColdWaterValue = 37,
            HotWaterValue = 29,
        };

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingWaterMeterReading);

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<MeterReading>()))
            .Returns(Task.CompletedTask);

        await _waterMeterReadingCommandService.UpdateMeterReadingAsync(updateWaterMeterReadingDto);

        existingWaterMeterReading.ColdWaterValue.Should().Be(37);
        existingWaterMeterReading.HotWaterValue.Should().Be(29);
        _waterMeterReadingRepositoryMock.Verify(repo => 
            repo.UpdateAsync(existingWaterMeterReading), Times.Once);
    }

    #endregion
    
    #region Update

    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.GetMeterReadingByIdAsync"/>, проверяющий успешное получение показателей водомеров
    /// </summary>
    [Fact]
    public async Task GetWaterMeterReadingByIdAsync_Should_Return_WaterMeterReading_When_Exists()
    {
        var userId = Guid.NewGuid();
        var existingWaterMeterReading = new MeterReading
        {
            Id = userId,
            ColdWaterValue = 34,
            HotWaterValue = 25,
            ReadingDate = DateTime.UtcNow
        };

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingWaterMeterReading);

        var result = await _waterMeterReadingCommandService.GetMeterReadingByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.ColdWaterValue.Should().Be(existingWaterMeterReading.ColdWaterValue);
        result.HotWaterValue.Should().Be(existingWaterMeterReading.HotWaterValue);
        _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.GetMeterReadingByIdAsync"/>, проверяющий отсутствие показателей водомеров
    /// </summary>
    [Fact]
    public async Task GetWaterMeterReadingByIdAsync_Should_Return_Null_When_WaterMeterReading_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((MeterReading)null);

        var result = await _waterMeterReadingCommandService.GetMeterReadingByIdAsync(userId);

        result.Should().BeNull();
        _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
    
    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.GetAllMeterReadingAsync"/>, проверяющий успешное получение всех показателей водомеров
    /// </summary>
    [Fact]
    public async Task GetAllWaterMeterReadingAsync_ShouldReturnAllWaterMeterReading()
    {
        var waterMeterReading = new List<MeterReading>
        {
            new MeterReading { Id = Guid.NewGuid(), ColdWaterValue = 20, HotWaterValue = 29 },
            new MeterReading { Id = Guid.NewGuid(), ColdWaterValue = 26, HotWaterValue = 17 }
        };

        _waterMeterReadingRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(waterMeterReading);

        var result = await _waterMeterReadingCommandService.GetAllMeterReadingAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(waterMeterReading);

        _waterMeterReadingRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    #endregion
    
    #region Delete

    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommandService.DeleteMeterReadingAsync"/>, проверяющий удаление показателей водомеров
    /// </summary>
    [Fact]
    public async Task DeleteWaterMeterReadingAsync_ShouldDeleteWaterMeterReading_WhenWaterMeterReadingExists()
    {
        var userId = Guid.NewGuid();
        var waterMeterReading = new MeterReading
        {
            Id = userId,
            ColdWaterValue = 34,
            HotWaterValue = 25,
            ReadingDate = DateTime.UtcNow
        };

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(waterMeterReading);

        _waterMeterReadingRepositoryMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<MeterReading>()))
            .Returns(Task.CompletedTask);

        await _waterMeterReadingCommandService.DeleteMeterReadingAsync(userId);

        _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _waterMeterReadingRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<MeterReading>(u => u.Id == userId)), Times.Once);
    }

    #endregion
}