using Application.Models.UsersModel;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators;

/// <summary>
/// Валидатор для модели добавления пользователя
/// </summary>
public class UserAddDtoValidator : AbstractValidator<UserAddDto>
{
    private static readonly Regex PhoneRegex = new(@"^\+7\d{10}$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", RegexOptions.Compiled);

    public UserAddDtoValidator()
    {
        RuleFor(x => x.ApartmentNumber)
            .NotEmpty().WithMessage("Номер квартиры не может быть пустым")
            .GreaterThan(0).WithMessage("Номер квартиры должен быть положительным числом");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия не может быть пустой")
            .MaximumLength(50).WithMessage("Фамилия не может быть длиннее 50 символов");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя не может быть пустым")
            .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов");

        RuleFor(x => x.MiddleName)
            .MaximumLength(50).WithMessage("Отчество не может быть длиннее 50 символов")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
            .Matches(PasswordRegex).WithMessage("Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона не может быть пустым")
            .Matches(PhoneRegex).WithMessage("Номер телефона должен быть в формате +7XXXXXXXXXX");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Указана недопустимая роль пользователя");
    }
}

/// <summary>
/// Валидатор для модели обновления пользователя
/// </summary>
public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    private static readonly Regex PhoneRegex = new(@"^\+7\d{10}$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", RegexOptions.Compiled);

    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым");

        When(x => x.ApartmentNumber.HasValue, () =>
        {
            RuleFor(x => x.ApartmentNumber)
                .GreaterThan(0).WithMessage("Номер квартиры должен быть положительным числом");
        });

        When(x => !string.IsNullOrEmpty(x.LastName), () =>
        {
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Фамилия не может быть длиннее 50 символов");
        });

        When(x => !string.IsNullOrEmpty(x.FirstName), () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов");
        });

        When(x => !string.IsNullOrEmpty(x.MiddleName), () =>
        {
            RuleFor(x => x.MiddleName)
                .MaximumLength(50).WithMessage("Отчество не может быть длиннее 50 символов");
        });

        When(x => !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
                .Matches(PasswordRegex).WithMessage("Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру");
        });

        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(PhoneRegex).WithMessage("Номер телефона должен быть в формате +7XXXXXXXXXX");
        });

        When(x => x.Role.HasValue, () =>
        {
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Указана недопустимая роль пользователя");
        });
    }
} 