using System;
using System.Threading.Tasks;
using Application.Models.WaterMeterModel;
using Application.Validators;
using Domain.Enums;
using Xunit;

namespace Tests.Validators;

/// <summary>
/// Тесты для <see cref="WaterMeterAddDtoValidator"/> и <see cref="WaterMeterUpdateDtoValidator"/>
/// </summary>
public class WaterMeterValidatorTests
{
    private readonly WaterMeterAddDtoValidator _addValidator;
    private readonly WaterMeterUpdateDtoValidator _updateValidator;

    /// <summary>
    /// Инициализирует тестовый контекст, создавая валидаторы.
    /// </summary>
    public WaterMeterValidatorTests()
    {
        _addValidator = new WaterMeterAddDtoValidator();
        _updateValidator = new WaterMeterUpdateDtoValidator();
    }

    /// <summary>
    /// Проверяет валидные данные для добавления счетчика воды
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldPass_WithValidDto()
    {
        // Arrange
        var dto = new WaterMeterAddDto
        {
            UserId = Guid.NewGuid(),
            PlaceOfWaterMeter = PlaceOfWaterMeter.Kitchen,
            WaterType = WaterType.Hot,
            FactoryNumber = "WM123456",
            FactoryYear = DateOnly.FromDateTime(DateTime.Now.AddYears(-1))
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Проверяет невалидность при пустом идентификаторе пользователя
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithEmptyUserId()
    {
        // Arrange
        var dto = new WaterMeterAddDto
        {
            UserId = Guid.Empty,
            PlaceOfWaterMeter = PlaceOfWaterMeter.Kitchen,
            WaterType = WaterType.Hot,
            FactoryNumber = "WM123456",
            FactoryYear = DateOnly.FromDateTime(DateTime.Now.AddYears(-1))
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UserId");
    }

    /// <summary>
    /// Проверяет невалидность при неправильном заводском номере
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithInvalidFactoryNumber()
    {
        // Arrange
        var dto = new WaterMeterAddDto
        {
            UserId = Guid.NewGuid(),
            PlaceOfWaterMeter = PlaceOfWaterMeter.Kitchen,
            WaterType = WaterType.Hot,
            FactoryNumber = "WM123456@#$", // Содержит спецсимволы
            FactoryYear = DateOnly.FromDateTime(DateTime.Now.AddYears(-1))
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FactoryNumber");
    }

    /// <summary>
    /// Проверяет невалидность при дате установки в будущем
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithFutureFactoryYear()
    {
        // Arrange
        var dto = new WaterMeterAddDto
        {
            UserId = Guid.NewGuid(),
            PlaceOfWaterMeter = PlaceOfWaterMeter.Kitchen,
            WaterType = WaterType.Hot,
            FactoryNumber = "WM123456",
            FactoryYear = DateOnly.FromDateTime(DateTime.Now.AddYears(1)) // Дата в будущем
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FactoryYear");
    }
} 