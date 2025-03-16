using System;
using System.Threading.Tasks;
using Application.Models.UsersModel;
using Application.Validators;
using Domain.Enums;
using Xunit;

namespace Tests.Validators;

/// <summary>
/// Тесты для <see cref="UserAddDtoValidator"/> и <see cref="UserUpdateDtoValidator"/>
/// </summary>
public class UserValidatorTests
{
    private readonly UserAddDtoValidator _addValidator;
    private readonly UserUpdateDtoValidator _updateValidator;

    /// <summary>
    /// Инициализирует тестовый контекст, создавая валидаторы.
    /// </summary>
    public UserValidatorTests()
    {
        _addValidator = new UserAddDtoValidator();
        _updateValidator = new UserUpdateDtoValidator();
    }

    /// <summary>
    /// Проверяет валидные данные для добавления пользователя
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldPass_WithValidDto()
    {
        // Arrange
        var dto = new UserAddDto
        {
            ApartmentNumber = 101,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "Password123",
            PhoneNumber = "+79001234567",
            Role = UserRole.User
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Проверяет невалидность при отрицательном номере квартиры
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithNegativeApartmentNumber()
    {
        // Arrange
        var dto = new UserAddDto
        {
            ApartmentNumber = -101,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "Password123",
            PhoneNumber = "+79001234567",
            Role = UserRole.User
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ApartmentNumber");
    }

    /// <summary>
    /// Проверяет невалидность при неправильном формате пароля
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithInvalidPassword()
    {
        // Arrange
        var dto = new UserAddDto
        {
            ApartmentNumber = 101,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "password", // Нет заглавных букв и цифр
            PhoneNumber = "+79001234567",
            Role = UserRole.User
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    /// <summary>
    /// Проверяет невалидность при неправильном формате телефона
    /// </summary>
    [Fact]
    public void ValidateAdd_ShouldFail_WithInvalidPhoneNumber()
    {
        // Arrange
        var dto = new UserAddDto
        {
            ApartmentNumber = 101,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "Password123",
            PhoneNumber = "89001234567", // Нет +7 в начале
            Role = UserRole.User
        };

        // Act
        var result = _addValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PhoneNumber");
    }

    /// <summary>
    /// Проверяет валидные данные для обновления пользователя
    /// </summary>
    [Fact]
    public void ValidateUpdate_ShouldPass_WithValidDto()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Id = Guid.NewGuid(),
            ApartmentNumber = 102,
            LastName = "Петров",
            FirstName = "Петр",
            Password = "Password456",
            PhoneNumber = "+79009876543"
        };

        // Act
        var result = _updateValidator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Проверяет невалидность при пустом идентификаторе пользователя
    /// </summary>
    [Fact]
    public void ValidateUpdate_ShouldFail_WithEmptyId()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Id = Guid.Empty,
            ApartmentNumber = 102,
            LastName = "Петров",
            FirstName = "Петр"
        };

        // Act
        var result = _updateValidator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
} 