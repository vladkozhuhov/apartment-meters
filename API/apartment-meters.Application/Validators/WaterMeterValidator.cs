using Application.Models.WaterMeterModel;
using FluentValidation;
using System.Text.RegularExpressions;
using Domain.Enums;

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
            .NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым");

        RuleFor(x => x.PlaceOfWaterMeter)
            .IsInEnum().WithMessage("Указано недопустимое расположение счетчика");

        RuleFor(x => x.WaterType)
            .IsInEnum().WithMessage("Указан недопустимый тип счетчика");

        RuleFor(x => x.FactoryNumber)
            .NotEmpty().WithMessage("Заводской номер счетчика не может быть пустым")
            .MaximumLength(10).WithMessage("Заводской номер счетчика не может быть длиннее 10 символов")
            .Matches(FactoryNumberRegex).WithMessage("Заводской номер должен содержать только буквы и цифры");

        RuleFor(x => x.FactoryYear)
            .NotEmpty().WithMessage("Дата установки счетчика не может быть пустой")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Дата установки счетчика не может быть в будущем");
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
            .NotEmpty().WithMessage("Идентификатор счетчика не может быть пустым");

        When(x => x.UserId.HasValue, () =>
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым");
        });

        When(x => x.PlaceOfWaterMeter.HasValue, () =>
        {
            RuleFor(x => x.PlaceOfWaterMeter)
                .IsInEnum().WithMessage("Указано недопустимое расположение счетчика");
        });

        When(x => x.WaterType.HasValue, () =>
        {
            RuleFor(x => x.WaterType)
                .IsInEnum().WithMessage("Указан недопустимый тип счетчика");
        });

        When(x => !string.IsNullOrEmpty(x.FactoryNumber), () =>
        {
            RuleFor(x => x.FactoryNumber)
                .MaximumLength(10).WithMessage("Заводской номер счетчика не может быть длиннее 10 символов")
                .Matches(FactoryNumberRegex).WithMessage("Заводской номер должен содержать только буквы и цифры");
        });

        When(x => x.FactoryYear.HasValue, () =>
        {
            RuleFor(x => x.FactoryYear)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Дата установки счетчика не может быть в будущем");
        });
    }
} 