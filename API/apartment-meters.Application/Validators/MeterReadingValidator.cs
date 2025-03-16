using System.Text.RegularExpressions;
using Application.Models.MeterReadingModel;
using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Валидатор для модели добавления показаний счетчика
/// </summary>
public class MeterReadingAddDtoValidator : AbstractValidator<MeterReadingAddDto>
{
    private static readonly Regex WaterValueRegex = new(@"^\d{1,5},\d{1,3}$", RegexOptions.Compiled);
    
    public MeterReadingAddDtoValidator()
    {
        RuleFor(x => x.WaterMeterId)
            .NotEmpty().WithMessage("Идентификатор счетчика не может быть пустым");
            
        RuleFor(x => x.WaterValue)
            .NotEmpty().WithMessage("Показание счетчика не может быть пустым")
            .MaximumLength(20).WithMessage("Показание счетчика не может быть длиннее 20 символов")
            .Matches(WaterValueRegex).WithMessage("Формат показаний должен содержать до 5 цифр до запятой и до 3 после");
            
        RuleFor(x => x.ReadingDate)
            .NotEmpty().WithMessage("Дата показания не может быть пустой")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Дата показания не может быть в будущем");
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
            .NotEmpty().WithMessage("Идентификатор счетчика не может быть пустым");
            
        RuleFor(x => x.WaterValue)
            .NotEmpty().WithMessage("Показание счетчика не может быть пустым")
            .MaximumLength(20).WithMessage("Показание счетчика не может быть длиннее 20 символов")
            .Matches(WaterValueRegex).WithMessage("Формат показаний должен содержать до 5 цифр до запятой и до 3 после");
    }
} 