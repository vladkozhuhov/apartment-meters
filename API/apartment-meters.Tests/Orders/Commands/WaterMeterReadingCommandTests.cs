using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Models;
using Application.Orders.Commands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Orders.Commands;

/// <summary>
/// Тесты для <see cref="WaterMeterReadingCommandTests"/>
/// </summary>
public class WaterMeterReadingCommandTests
{
    /// <summary>
    /// Мок-объект для репозитория водомеров
    /// </summary>
    private readonly Mock<IWaterMeterReadingRepository> _waterMeterReadingRepositoryMock;

    /// <summary>
    /// Сервис для выполнения команд водомеров
    /// </summary>
    private readonly IWaterMeterReadingCommand _waterMeterReadingCommand;
    
    /// <summary>
    /// Сервис для извлечения данных водомеров
    /// </summary>
    private readonly IWaterMeterReadingQuery _waterMeterReadingQuery;

    /// <summary>
    /// Инициализация тестового класса <see cref="WaterMeterReadingCommandTests"/>
    /// </summary>
    public WaterMeterReadingCommandTests()
    {
        _waterMeterReadingRepositoryMock = new Mock<IWaterMeterReadingRepository>();
        _waterMeterReadingCommand = new WaterMeterReadingCommand(_waterMeterReadingRepositoryMock.Object);
    }

    // #region Add

    /// <summary>
    /// Тест метода <see cref="IWaterMeterReadingCommand.AddMeterReadingAsync"/>, проверяющий успешное добавление показателей водомеров
    /// </summary>
    // [Fact]
    // public async Task AddMeterReadingAsync_Should_Add_WaterMeterReading_Successfully()
    // {
    //     var newWaterMeterReading = new AddWaterMeterReadingDto()
    //     {
    //         ColdWaterValue = 34,
    //         HotWaterValue = 25,
    //         ReadingDate = DateTime.UtcNow
    //     };
    //     
    //     var waterMeterReading = new MeterReadingEntity()
    //     {
    //         Id = Guid.NewGuid(),
    //         ColdWaterValue = newWaterMeterReading.ColdWaterValue,
    //         HotWaterValue = newWaterMeterReading.HotWaterValue,
    //         ReadingDate = newWaterMeterReading.ReadingDate
    //     };
    //     
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.AddAsync(It.IsAny<MeterReadingEntity>()))
    //         .ReturnsAsync(waterMeterReading);
    //     
    //     var result = await _waterMeterReadingCommand.AddMeterReadingAsync(newWaterMeterReading);
    //     
    //     result.Should().NotBeNull();
    //     result.ColdWaterValue.Should().Be(newWaterMeterReading.ColdWaterValue);
    //     result.HotWaterValue.Should().Be(newWaterMeterReading.HotWaterValue);
    //     _waterMeterReadingRepositoryMock.Verify(repo =>
    //         repo.AddAsync(It.Is<MeterReadingEntity>(u =>
    //             u.ColdWaterValue == newWaterMeterReading.ColdWaterValue &&
    //             u.ColdWaterValue == newWaterMeterReading.ColdWaterValue &&
    //             u.HotWaterValue == newWaterMeterReading.HotWaterValue)), Times.Once);
    // }
    //
    // #endregion
    //
    // #region Update
    //
    // /// <summary>
    // /// Тест метода <see cref="IWaterMeterReadingCommand.UpdateMeterReadingAsync"/>, проверяющий успешное обновление показателей водомеров
    // /// </summary>
    // [Fact]
    // public async Task UpdateWaterMeterReadingAsync_Should_Update_WaterMeterReading_Successfully()
    // {
    //     var userId = Guid.NewGuid();
    //     var existingWaterMeterReading = new MeterReadingEntity()
    //     {
    //         Id = userId,
    //         ColdWaterValue = 34,
    //         HotWaterValue = 25,
    //         ReadingDate = DateTime.UtcNow
    //     };
    //
    //     var updateWaterMeterReadingDto = new UpdateWaterMeterReadingDto()
    //     {
    //         UserId = userId,
    //         ColdWaterValue = 37,
    //         HotWaterValue = 29,
    //     };
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.GetByIdAsync(userId))
    //         .ReturnsAsync(existingWaterMeterReading);
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.UpdateAsync(It.IsAny<MeterReadingEntity>()))
    //         .Returns(Task.CompletedTask);
    //
    //     await _waterMeterReadingCommand.UpdateMeterReadingAsync(userId, updateWaterMeterReadingDto);
    //
    //     existingWaterMeterReading.ColdWaterValue.Should().Be(37);
    //     existingWaterMeterReading.HotWaterValue.Should().Be(29);
    //     _waterMeterReadingRepositoryMock.Verify(repo => 
    //         repo.UpdateAsync(existingWaterMeterReading), Times.Once);
    // }
    //
    // #endregion
    //
    // #region Get
    //
    // /// <summary>
    // /// Тест метода <see cref="IWaterMeterReadingQuery.GetMeterReadingByIdAsync"/>, проверяющий успешное получение показателей водомеров по идентификатору
    // /// </summary>
    // [Fact]
    // public async Task GetWaterMeterReadingByIdAsync_Should_Return_WaterMeterReading_When_Exists()
    // {
    //     var id = Guid.NewGuid();
    //     var existingWaterMeterReading = new MeterReadingEntity
    //     {
    //         Id = id,
    //         ColdWaterValue = 34,
    //         HotWaterValue = 25,
    //         ReadingDate = DateTime.UtcNow
    //     };
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.GetByIdAsync(id))
    //         .ReturnsAsync(existingWaterMeterReading);
    //
    //     var result = await _waterMeterReadingQuery.GetMeterReadingByIdAsync(id);
    //
    //     result.Should().NotBeNull();
    //     result.Id.Should().Be(id);
    //     result.ColdWaterValue.Should().Be(existingWaterMeterReading.ColdWaterValue);
    //     result.HotWaterValue.Should().Be(existingWaterMeterReading.HotWaterValue);
    //     _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    // }
    //
    // /// <summary>
    // /// Тест метода <see cref="IWaterMeterReadingQuery.GetMeterReadingByUserIdAsync"/>, проверяющий отсутствие показателей водомеров по пользователю
    // /// </summary>
    // [Fact]
    // public async Task GetWaterMeterReadingByUserIdAsync_Should_Return_Null_When_WaterMeterReading_Does_Not_Exist()
    // {
    //     var userId = Guid.NewGuid();
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.GetByIdAsync(userId))
    //         .ReturnsAsync((MeterReadingEntity)null);
    //
    //     var result = await _waterMeterReadingQuery.GetMeterReadingByUserIdAsync(userId);
    //
    //     result.Should().BeNull();
    //     _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    // }
    //
    // /// <summary>
    // /// Тест метода <see cref="IWaterMeterReadingQuery.GetAllMeterReadingAsync"/>, проверяющий успешное получение всех показателей водомеров
    // /// </summary>
    // [Fact]
    // public async Task GetAllWaterMeterReadingAsync_ShouldReturnAllWaterMeterReading()
    // {
    //     var waterMeterReading = new List<MeterReadingEntity>
    //     {
    //         new MeterReadingEntity { Id = Guid.NewGuid(), ColdWaterValue = 20, HotWaterValue = 29 },
    //         new MeterReadingEntity { Id = Guid.NewGuid(), ColdWaterValue = 26, HotWaterValue = 17 }
    //     };
    //
    //     _waterMeterReadingRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(waterMeterReading);
    //
    //     var result = await _waterMeterReadingQuery.GetAllMeterReadingAsync();
    //
    //     result.Should().NotBeNull();
    //     result.Should().HaveCount(2);
    //     result.Should().BeEquivalentTo(waterMeterReading);
    //
    //     _waterMeterReadingRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    // }
    //
    // #endregion
    //
    // #region Delete
    //
    // /// <summary>
    // /// Тест метода <see cref="IWaterMeterReadingCommand.DeleteMeterReadingAsync"/>, проверяющий удаление показателей водомеров
    // /// </summary>
    // [Fact]
    // public async Task DeleteWaterMeterReadingAsync_ShouldDeleteWaterMeterReading_WhenWaterMeterReadingExists()
    // {
    //     var userId = Guid.NewGuid();
    //     var waterMeterReading = new MeterReadingEntity
    //     {
    //         Id = userId,
    //         ColdWaterValue = 34,
    //         HotWaterValue = 25,
    //         ReadingDate = DateTime.UtcNow
    //     };
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.GetByIdAsync(userId))
    //         .ReturnsAsync(waterMeterReading);
    //
    //     _waterMeterReadingRepositoryMock
    //         .Setup(repo => repo.DeleteAsync(It.IsAny<MeterReadingEntity>()))
    //         .Returns(Task.CompletedTask);
    //
    //     await _waterMeterReadingCommand.DeleteMeterReadingAsync(userId);
    //
    //     _waterMeterReadingRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    //     _waterMeterReadingRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<MeterReadingEntity>(u => u.Id == userId)), Times.Once);
    // }
    //
    // #endregion
}