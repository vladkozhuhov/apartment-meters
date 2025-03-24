using Application.Behaviors;
using Application.Services;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace Tests.Behaviors;

/// <summary>
/// Тесты для ValidationBehavior
/// </summary>
public class ValidationBehaviorTests
{
    private readonly Mock<IErrorHandlingService> _errorHandlingServiceMock;
    private readonly ValidationBehavior<TestRequest, TestResponse> _behavior;

    public ValidationBehaviorTests()
    {
        _errorHandlingServiceMock = new Mock<IErrorHandlingService>();
        _behavior = new ValidationBehavior<TestRequest, TestResponse>(
            new List<IValidator<TestRequest>>(),
            _errorHandlingServiceMock.Object);
    }

    /// <summary>
    /// Проверяет, что при отсутствии валидаторов вызывается следующий делегат
    /// </summary>
    [Fact]
    public async Task Handle_WithoutValidators_CallsNextDelegate()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = new TestResponse();
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse, result);
        _errorHandlingServiceMock.Verify(s => s.ThrowValidationException(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }

    /// <summary>
    /// Проверяет, что при валидном запросе вызывается следующий делегат
    /// </summary>
    [Fact]
    public async Task Handle_WithValidRequest_CallsNextDelegate()
    {
        // Arrange
        var request = new TestRequest { Property = "Valid" };
        var validator = new TestRequestValidator();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            new List<IValidator<TestRequest>> { validator },
            _errorHandlingServiceMock.Object);
        var expectedResponse = new TestResponse();
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse, result);
        _errorHandlingServiceMock.Verify(s => s.ThrowValidationException(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }

    /// <summary>
    /// Проверяет, что при невалидном запросе вызывается ThrowValidationException
    /// </summary>
    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new TestRequest { Property = "" };
        var validator = new TestRequestValidator();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            new List<IValidator<TestRequest>> { validator },
            _errorHandlingServiceMock.Object);
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(new TestResponse());

        // Act
        await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        _errorHandlingServiceMock.Verify(s => s.ThrowValidationException(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
    }

    /// <summary>
    /// Проверяет, что при наличии нескольких валидаторов все они применяются
    /// </summary>
    [Fact]
    public async Task Handle_Should_ValidateAllValidators_When_MultipleValidatorsExist()
    {
        // Arrange
        var request = new TestRequest { Property = "test" };
        var validators = new List<IValidator<TestRequest>>
        {
            new TestRequestValidator(),
            new TestRequestValidator2()
        };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators, _errorHandlingServiceMock.Object);

        // Act
        await behavior.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

        // Assert
        _errorHandlingServiceMock.Verify(x => x.ThrowValidationException(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }
}

public class TestRequest : IRequest<TestResponse>
{
    public string Property { get; set; } = string.Empty;
}

public class TestResponse
{
}

public class TestRequestValidator : AbstractValidator<TestRequest>
{
    public TestRequestValidator()
    {
        RuleFor(x => x.Property).NotEmpty();
    }
}

public class TestRequestValidator2 : AbstractValidator<TestRequest>
{
    public TestRequestValidator2()
    {
        RuleFor(x => x.Property).MinimumLength(3);
    }
} 