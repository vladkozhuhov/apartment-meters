using Application.Interfaces.Commands;
using Application.Orders.Commands;
using Domain.Repositories;
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

    

    #endregion
    
    #region Get

    

    #endregion
    
    #region Update

    

    #endregion
    
    #region Delete

    

    #endregion
}