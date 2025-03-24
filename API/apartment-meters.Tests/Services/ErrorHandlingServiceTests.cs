using Application.Exceptions;
using Application.Services;
using Domain.Enums;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Services;

public class ErrorHandlingServiceTests
{
    private readonly Mock<ILogger<ErrorHandlingService>> _loggerMock;
    private readonly ErrorHandlingService _service;

    public ErrorHandlingServiceTests()
    {
        _loggerMock = new Mock<ILogger<ErrorHandlingService>>();
        _service = new ErrorHandlingService(_loggerMock.Object);
    }

    [Fact]
    public void ThrowBusinessLogicException_WithErrorType_ThrowsExceptionWithCorrectError()
    {
        // Arrange
        var errorType = ErrorType.InvalidPasswordError102;

        // Act & Assert
        var exception = Assert.Throws<BusinessLogicException>(() => 
            _service.ThrowBusinessLogicException(errorType));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(errorType.GetMessage());
    }

    [Fact]
    public void ThrowBusinessLogicException_WithCustomMessage_ThrowsExceptionWithCustomMessage()
    {
        // Arrange
        var errorType = ErrorType.InvalidPasswordError102;
        var customMessage = "Пользовательское сообщение об ошибке";

        // Act & Assert
        var exception = Assert.Throws<BusinessLogicException>(() => 
            _service.ThrowBusinessLogicException(errorType, customMessage));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void ThrowNotFoundException_WithErrorType_ThrowsExceptionWithCorrectError()
    {
        // Arrange
        var errorType = ErrorType.WaterMeterNotFoundError351;

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() => 
            _service.ThrowNotFoundException(errorType));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(errorType.GetMessage());
    }

    [Fact]
    public void ThrowNotFoundException_WithCustomMessage_ThrowsExceptionWithCustomMessage()
    {
        // Arrange
        var errorType = ErrorType.WaterMeterNotFoundError351;
        var customMessage = "Счётчик воды не найден";

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() => 
            _service.ThrowNotFoundException(errorType, customMessage));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void ThrowForbiddenAccessException_WithErrorType_ThrowsExceptionWithCorrectError()
    {
        // Arrange
        var errorType = ErrorType.UserPermissionDeniedError203;

        // Act & Assert
        var exception = Assert.Throws<ForbiddenAccessException>(() => 
            _service.ThrowForbiddenAccessException(errorType));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(errorType.GetMessage());
    }

    [Fact]
    public void ThrowForbiddenAccessException_WithCustomMessage_ThrowsExceptionWithCustomMessage()
    {
        // Arrange
        var errorType = ErrorType.UserPermissionDeniedError203;
        var customMessage = "У вас нет доступа к этому ресурсу";

        // Act & Assert
        var exception = Assert.Throws<ForbiddenAccessException>(() => 
            _service.ThrowForbiddenAccessException(errorType, customMessage));
        
        exception.ErrorType.Should().Be(errorType);
        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void ThrowValidationException_WithValidationFailures_ThrowsFluentValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Property1", "Ошибка валидации 1") 
            { 
                ErrorCode = ErrorType.EmptyWaterValueError480.ToString() 
            },
            new ValidationFailure("Property2", "Ошибка валидации 2") 
            { 
                ErrorCode = ErrorType.InvalidWaterValueFormatError481.ToString() 
            }
        };

        // Act & Assert
        var exception = Assert.Throws<FluentValidation.ValidationException>(() => 
            _service.ThrowValidationException(failures));
        
        exception.Errors.Should().HaveCount(2);
        exception.Errors.Select(e => e.ErrorMessage).Should()
            .Contain("Ошибка валидации 1")
            .And.Contain("Ошибка валидации 2");
    }
} 