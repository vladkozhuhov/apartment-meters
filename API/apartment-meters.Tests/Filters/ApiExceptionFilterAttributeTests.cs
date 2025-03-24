using System.Net;
using API.Filters;
using Application.Exceptions;
using Domain.Enums;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Filters;

public class ApiExceptionFilterAttributeTests
{
    private readonly Mock<ILogger<ApiExceptionFilterAttribute>> _loggerMock;
    private readonly ApiExceptionFilterAttribute _filter;
    
    public ApiExceptionFilterAttributeTests()
    {
        _loggerMock = new Mock<ILogger<ApiExceptionFilterAttribute>>();
        _filter = new ApiExceptionFilterAttribute(_loggerMock.Object);
    }
    
    [Fact]
    public void OnException_WithValidationException_ReturnsBadRequestWithValidationDetails()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Property1", "Ошибка валидации 1"),
            new ValidationFailure("Property2", "Ошибка валидации 2")
        };
        var exception = new ValidationException(failures);
        var context = CreateExceptionContext(exception);
        
        // Act
        _filter.OnException(context);
        
        // Assert
        var result = context.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        
        var problemDetails = result.Value as ValidationProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problemDetails.Title.Should().Be("Ошибка валидации запроса");
        problemDetails.Errors.Should().ContainKey("Property1");
        problemDetails.Errors.Should().ContainKey("Property2");
    }
    
    [Fact]
    public void OnException_WithNotFoundException_ReturnsNotFoundWithErrorDetails()
    {
        // Arrange
        var errorType = ErrorType.WaterMeterNotFoundError351;
        var message = "Счётчик воды не найден";
        var exception = new NotFoundException(message, errorType);
        var context = CreateExceptionContext(exception);
        
        // Act
        _filter.OnException(context);
        
        // Assert
        var result = context.Result as NotFoundObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.NotFound);
        problemDetails.Title.Should().Be("Ресурс не найден");
        problemDetails.Detail.Should().Be(message);
        
        problemDetails.Extensions.Should().ContainKey("errorCode");
        problemDetails.Extensions["errorCode"].Should().Be((int)errorType);
        problemDetails.Extensions.Should().ContainKey("errorType");
        problemDetails.Extensions["errorType"].ToString().Should().Contain(errorType.ToString());
    }
    
    [Fact]
    public void OnException_WithBusinessLogicException_ReturnsBadRequestWithErrorDetails()
    {
        // Arrange
        var errorType = ErrorType.InvalidPasswordError102;
        var message = "Неверный пароль";
        var exception = new BusinessLogicException(errorType, message);
        var context = CreateExceptionContext(exception);
        
        // Act
        _filter.OnException(context);
        
        // Assert
        var result = context.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problemDetails.Title.Should().Be("Ошибка бизнес-логики");
        problemDetails.Detail.Should().Be(message);
        
        problemDetails.Extensions.Should().ContainKey("errorCode");
        problemDetails.Extensions["errorCode"].Should().Be((int)errorType);
        problemDetails.Extensions.Should().ContainKey("errorType");
        problemDetails.Extensions["errorType"].ToString().Should().Contain(errorType.ToString());
    }
    
    [Fact]
    public void OnException_WithForbiddenAccessException_ReturnsForbiddenWithErrorDetails()
    {
        // Arrange
        var errorType = ErrorType.UserPermissionDeniedError203;
        var message = "У вас нет прав для выполнения этого действия";
        var exception = new ForbiddenAccessException(message, errorType);
        var context = CreateExceptionContext(exception);
        
        // Act
        _filter.OnException(context);
        
        // Assert
        var result = context.Result as ObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.Forbidden);
        problemDetails.Title.Should().Be("Доступ запрещён");
        problemDetails.Detail.Should().Be(message);
        
        problemDetails.Extensions.Should().ContainKey("errorCode");
        problemDetails.Extensions["errorCode"].Should().Be((int)errorType);
        problemDetails.Extensions.Should().ContainKey("errorType");
        problemDetails.Extensions["errorType"].ToString().Should().Contain(errorType.ToString());
    }
    
    [Fact]
    public void OnException_WithUnknownException_ReturnsInternalServerError()
    {
        // Arrange
        var message = "Неизвестная ошибка";
        var exception = new Exception(message);
        var context = CreateExceptionContext(exception);
        
        // Act
        _filter.OnException(context);
        
        // Assert
        var result = context.Result as ObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        
        var problemDetails = result.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        problemDetails.Title.Should().Be("Внутренняя ошибка сервера");
    }
    
    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        };
        
        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }
} 