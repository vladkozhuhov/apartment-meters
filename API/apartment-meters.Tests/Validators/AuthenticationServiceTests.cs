using Application.Exceptions;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Models.LoginModels;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Validators;

/// <summary>
/// Тесты для <see cref="AuthenticationService"/>
/// </summary>
public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICachedRepository<UserEntity, Guid>> _cachedRepositoryMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// Инициализирует тестовый контекст, создавая моки и сервис аутентификации.
    /// </summary>
    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _cachedRepositoryMock = new Mock<ICachedRepository<UserEntity, Guid>>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _authService = new AuthenticationService(
            _userRepositoryMock.Object, 
            _cachedRepositoryMock.Object,
            _loggerMock.Object);
    }

    /// <summary>
    /// Проверяет успешную аутентификацию пользователя.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            ApartmentNumber = 101,
            Password = "correctPassword"
        };

        var expectedUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            ApartmentNumber = 101,
            LastName = "Иванов",
            FirstName = "Иван",
            MiddleName = "Иванович",
            Password = "correctPassword",
            Role = UserRole.User
        };

        _userRepositoryMock.Setup(repo => repo.GetByApartmentNumberAsync(loginDto.ApartmentNumber))
            .ReturnsAsync(expectedUser);
        
        _cachedRepositoryMock.Setup(repo => repo.GetByIdCachedAsync(expectedUser.Id, It.IsAny<int>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.ApartmentNumber, result.ApartmentNumber);
        
        // Проверяем, что был вызов к кэшу
        _cachedRepositoryMock.Verify(repo => repo.GetByIdCachedAsync(expectedUser.Id, It.IsAny<int>()), Times.Once);
    }

    /// <summary>
    /// Проверяет неудачную аутентификацию из-за некорректного пароля.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            ApartmentNumber = 102,
            Password = "wrongPassword"
        };

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            ApartmentNumber = 102,
            Password = "correctPassword"
        };

        _userRepositoryMock.Setup(repo => repo.GetByApartmentNumberAsync(loginDto.ApartmentNumber))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessLogicException>(
            () => _authService.LoginAsync(loginDto));
        
        Assert.Equal(ErrorType.InvalidPasswordError102, exception.ErrorType);
    }

    /// <summary>
    /// Проверяет неудачную аутентификацию из-за несуществующего номера квартиры.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            ApartmentNumber = 999,
            Password = "anyPassword"
        };

        _userRepositoryMock.Setup(repo => repo.GetByApartmentNumberAsync(loginDto.ApartmentNumber))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessLogicException>(
            () => _authService.LoginAsync(loginDto));
        
        Assert.Equal(ErrorType.UserNotFoundError101, exception.ErrorType);
    }
} 