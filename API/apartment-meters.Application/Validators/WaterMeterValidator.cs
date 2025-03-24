using Application.Models.WaterMeterModel;
using FluentValidation;
using System.Text.RegularExpressions;
using Domain.Enums;
using Application.Exceptions;

namespace Application.Validators;

/// <summary>
/// Валидатор для модели добавления счетчика воды
/// </summary>
public class WaterMeterAddDtoValidator : AbstractValidator<WaterMeterAddDto>
{
    private static readonly Regex FactoryNumberRegex = new(@"^[a-zA-Z0-9]{1,10}$", RegexOptions.Compiled);

    public WaterMeterAddDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithErrorCode(ErrorType.EmptyUserIdError463.ToString())
            .WithMessage(ErrorType.EmptyUserIdError463.GetMessage());

        RuleFor(x => x.PlaceOfWaterMeter)
            .IsInEnum().WithErrorCode(ErrorType.InvalidWaterMeterPlaceError471.ToString())
            .WithMessage(ErrorType.InvalidWaterMeterPlaceError471.GetMessage());

        RuleFor(x => x.WaterType)
            .IsInEnum().WithErrorCode(ErrorType.InvalidWaterTypeError472.ToString())
            .WithMessage(ErrorType.InvalidWaterTypeError472.GetMessage());

        RuleFor(x => x.FactoryNumber)
            .NotEmpty().WithErrorCode(ErrorType.EmptyFactoryNumberError473.ToString())
            .WithMessage(ErrorType.EmptyFactoryNumberError473.GetMessage())
            .MaximumLength(10).WithErrorCode(ErrorType.FactoryNumberTooLongError474.ToString())
            .WithMessage(ErrorType.FactoryNumberTooLongError474.GetMessage())
            .Matches(FactoryNumberRegex).WithErrorCode(ErrorType.InvalidFactoryNumberFormatError475.ToString())
            .WithMessage(ErrorType.InvalidFactoryNumberFormatError475.GetMessage());

        RuleFor(x => x.FactoryYear)
            .NotEmpty().WithErrorCode(ErrorType.EmptyFactoryYearError476.ToString())
            .WithMessage(ErrorType.EmptyFactoryYearError476.GetMessage())
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
            .WithErrorCode(ErrorType.FutureFactoryYearError477.ToString())
            .WithMessage(ErrorType.FutureFactoryYearError477.GetMessage());
    }
}

/// <summary>
/// Валидатор для модели обновления счетчика воды
/// </summary>
public class WaterMeterUpdateDtoValidator : AbstractValidator<WaterMeterUpdateDto>
{
    private static readonly Regex FactoryNumberRegex = new(@"^[a-zA-Z0-9]{1,10}$", RegexOptions.Compiled);

    public WaterMeterUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithErrorCode(ErrorType.EmptyWaterMeterIdError470.ToString())
            .WithMessage(ErrorType.EmptyWaterMeterIdError470.GetMessage());

        When(x => x.UserId.HasValue, () =>
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithErrorCode(ErrorType.EmptyUserIdError463.ToString())
                .WithMessage(ErrorType.EmptyUserIdError463.GetMessage());
        });

        When(x => x.PlaceOfWaterMeter.HasValue, () =>
        {
            RuleFor(x => x.PlaceOfWaterMeter)
                .IsInEnum().WithErrorCode(ErrorType.InvalidWaterMeterPlaceError471.ToString())
                .WithMessage(ErrorType.InvalidWaterMeterPlaceError471.GetMessage());
        });

        When(x => x.WaterType.HasValue, () =>
        {
            RuleFor(x => x.WaterType)
                .IsInEnum().WithErrorCode(ErrorType.InvalidWaterTypeError472.ToString())
                .WithMessage(ErrorType.InvalidWaterTypeError472.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.FactoryNumber), () =>
        {
            RuleFor(x => x.FactoryNumber)
                .MaximumLength(10).WithErrorCode(ErrorType.FactoryNumberTooLongError474.ToString())
                .WithMessage(ErrorType.FactoryNumberTooLongError474.GetMessage())
                .Matches(FactoryNumberRegex).WithErrorCode(ErrorType.InvalidFactoryNumberFormatError475.ToString())
                .WithMessage(ErrorType.InvalidFactoryNumberFormatError475.GetMessage());
        });

        When(x => x.FactoryYear.HasValue, () =>
        {
            RuleFor(x => x.FactoryYear)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
                .WithErrorCode(ErrorType.FutureFactoryYearError477.ToString())
                .WithMessage(ErrorType.FutureFactoryYearError477.GetMessage());
        });
    }
} 