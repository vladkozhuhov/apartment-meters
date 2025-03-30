using System.Text.RegularExpressions;
using Application.Models.MeterReadingModel;
using FluentValidation;
using Domain.Enums;
using Application.Exceptions;
using Application.Services;

namespace Application.Validators;

/// <summary>
/// Валидатор для модели добавления показаний счетчика
/// </summary>
public class MeterReadingAddDtoValidator : AbstractValidator<MeterReadingAddDto>
{
    private static readonly Regex WaterValueRegex = new(@"^\d{1,5},\d{1,3}$", RegexOptions.Compiled);
    private readonly IErrorHandlingService _errorHandlingService;
    
    public MeterReadingAddDtoValidator(IErrorHandlingService errorHandlingService = null)
    {
        _errorHandlingService = errorHandlingService;
        
        RuleFor(x => x.WaterMeterId)
            .NotEmpty().WithErrorCode(ErrorType.EmptyWaterMeterIdError470.ToString())
            .WithMessage(ErrorType.EmptyWaterMeterIdError470.GetMessage());
            
        RuleFor(x => x.WaterValue)
            .NotEmpty().WithErrorCode(ErrorType.EmptyWaterValueError480.ToString())
            .WithMessage(ErrorType.EmptyWaterValueError480.GetMessage())
            .Matches(WaterValueRegex).WithErrorCode(ErrorType.InvalidWaterValueFormatError481.ToString())
            .WithMessage(ErrorType.InvalidWaterValueFormatError481.GetMessage());
            
        RuleFor(x => x.ReadingDate)
            .NotEmpty().WithErrorCode(ErrorType.EmptyReadingDateError482.ToString())
            .WithMessage(ErrorType.EmptyReadingDateError482.GetMessage())
            .LessThanOrEqualTo(DateTime.UtcNow).WithErrorCode(ErrorType.FutureReadingDateError483.ToString())
            .WithMessage(ErrorType.FutureReadingDateError483.GetMessage())
            .Custom((date, context) =>
            {
                var day = date.Day;
                var today = DateTime.UtcNow.Date;
                
                // Показания можно отправлять только с 23 по 25 число месяца или в текущий день для тестирования
                if (!((day >= 23 && day <= 25) /*|| (date.Date == today)*/))
                {
                    // Если сервис обработки ошибок доступен, выбрасываем бизнес-ошибку
                    if (_errorHandlingService != null)
                    {
                        _errorHandlingService.ThrowBusinessLogicException(
                            ErrorType.MeterReadingOutsideAllowedPeriodError484,
                            "Показания можно подавать только с 23 по 25 число месяца.");
                    }
                    
                    // Обычная ошибка валидации как запасной вариант
                    context.AddFailure(nameof(MeterReadingAddDto.ReadingDate), 
                        ErrorType.MeterReadingOutsideAllowedPeriodError484.GetMessage());
                }
            });
    }
}

/// <summary>
/// Валидатор для модели обновления показаний счетчика
/// </summary>
public class MeterReadingUpdateDtoValidator : AbstractValidator<MeterReadingUpdateDto>
{
    private static readonly Regex WaterValueRegex = new(@"^\d{1,5},\d{1,3}$", RegexOptions.Compiled);
    
    public MeterReadingUpdateDtoValidator()
    {
        RuleFor(x => x.WaterMeterId)
            .NotEmpty().WithErrorCode(ErrorType.EmptyWaterMeterIdError470.ToString())
            .WithMessage(ErrorType.EmptyWaterMeterIdError470.GetMessage());
            
        RuleFor(x => x.WaterValue)
            .NotEmpty().WithErrorCode(ErrorType.EmptyWaterValueError480.ToString())
            .WithMessage(ErrorType.EmptyWaterValueError480.GetMessage())
            .Matches(WaterValueRegex).WithErrorCode(ErrorType.InvalidWaterValueFormatError481.ToString())
            .WithMessage(ErrorType.InvalidWaterValueFormatError481.GetMessage());
    }
} 