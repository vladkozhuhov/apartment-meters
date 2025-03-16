using System;
using System.Threading.Tasks;
using Application.Models.MeterReadingModel;
using Application.Validators;
using Xunit;

namespace Tests.Validators;

/// <summary>
/// Тесты для <see cref="MeterReadingAddDtoValidator"/> и <see cref="MeterReadingUpdateDtoValidator"/>
/// </summary>
public class MeterReadingValidatorTests
{
    private readonly MeterReadingAddDtoValidator _addValidator;
    private readonly MeterReadingUpdateDtoValidator _updateValidator;

    /// <summary>
    /// Инициализирует тестовый контекст, создавая валидаторы.
    /// </summary>
    public MeterReadingValidatorTests()
    {
        _addValidator = new MeterReadingAddDtoValidator();
        _updateValidator = new MeterReadingUpdateDtoValidator();
    }

    /// <summary>
    /// Проверяет валидные данные для добавления показаний счетчика
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldPass_WithValidDto()
    {
        // Arrange
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123,45",
            ReadingDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Проверяет невалидность при пустом идентификаторе счетчика
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithEmptyWaterMeterId()
    {
        // Arrange
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.Empty,
            WaterValue = "123,45",
            ReadingDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WaterMeterId");
    }

    /// <summary>
    /// Проверяет невалидность при неправильном формате показаний
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithInvalidWaterValueFormat()
    {
        // Arrange
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "12345", // Нет запятой
            ReadingDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WaterValue");
    }

    /// <summary>
    /// Проверяет невалидность при слишком большом значении показаний
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithTooLargeWaterValue()
    {
        // Arrange
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123456,789", // Больше 5 цифр до запятой
            ReadingDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WaterValue");
    }

    /// <summary>
    /// Проверяет невалидность при дате показаний в будущем
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithFutureReadingDate()
    {
        // Arrange
        var dto = new MeterReadingAddDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123,45",
            ReadingDate = DateTime.UtcNow.AddDays(1) // Дата в будущем
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReadingDate");
    }

    /// <summary>
    /// Проверяет валидные данные для обновления показаний счетчика
    /// </summary>
    [Fact]
    public void ValidateUpdate_ShouldPass_WithValidDto()
    {
        // Arrange
        var dto = new MeterReadingUpdateDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123,45"
        };

        // Act
        var result = _updateValidator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Проверяет невалидность при неправильном формате показаний при обновлении
    /// </summary>
    [Fact]
    public void ValidateUpdate_ShouldFail_WithInvalidWaterValueFormat()
    {
        // Arrange
        var dto = new MeterReadingUpdateDto
        {
            WaterMeterId = Guid.NewGuid(),
            WaterValue = "123.45" // Точка вместо запятой
        };

        // Act
        var result = _updateValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WaterValue");
    }
} 