using Domain.Enums;

namespace Application.Exceptions;

public class CustomException : Exception
{
    public ErrorType ErrorType { get; }

    public CustomException(ErrorType errorType) : base(errorType.GetMessage())
    {
        ErrorType = errorType;
    }
}