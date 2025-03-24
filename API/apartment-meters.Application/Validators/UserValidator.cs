using Application.Models.UsersModel;
using FluentValidation;
using System.Text.RegularExpressions;
using Domain.Enums;
using Application.Exceptions;

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
            .NotEmpty().WithErrorCode(ErrorType.EmptyApartmentNumberError450.ToString())
            .WithMessage(ErrorType.EmptyApartmentNumberError450.GetMessage())
            .GreaterThan(0).WithErrorCode(ErrorType.InvalidApartmentNumberError451.ToString())
            .WithMessage(ErrorType.InvalidApartmentNumberError451.GetMessage());

        RuleFor(x => x.LastName)
            .NotEmpty().WithErrorCode(ErrorType.EmptyLastNameError452.ToString())
            .WithMessage(ErrorType.EmptyLastNameError452.GetMessage())
            .MaximumLength(50).WithErrorCode(ErrorType.LastNameTooLongError453.ToString())
            .WithMessage(ErrorType.LastNameTooLongError453.GetMessage());

        RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(ErrorType.EmptyFirstNameError454.ToString())
            .WithMessage(ErrorType.EmptyFirstNameError454.GetMessage())
            .MaximumLength(50).WithErrorCode(ErrorType.FirstNameTooLongError455.ToString())
            .WithMessage(ErrorType.FirstNameTooLongError455.GetMessage());

        RuleFor(x => x.MiddleName)
            .MaximumLength(50).WithErrorCode(ErrorType.MiddleNameTooLongError456.ToString())
            .WithMessage(ErrorType.MiddleNameTooLongError456.GetMessage())
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ErrorType.EmptyPasswordError457.ToString())
            .WithMessage(ErrorType.EmptyPasswordError457.GetMessage())
            .MinimumLength(8).WithErrorCode(ErrorType.PasswordTooShortError458.ToString())
            .WithMessage(ErrorType.PasswordTooShortError458.GetMessage())
            .Matches(PasswordRegex).WithErrorCode(ErrorType.InvalidPasswordFormatError459.ToString())
            .WithMessage(ErrorType.InvalidPasswordFormatError459.GetMessage());

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithErrorCode(ErrorType.EmptyPhoneNumberError460.ToString())
            .WithMessage(ErrorType.EmptyPhoneNumberError460.GetMessage())
            .Matches(PhoneRegex).WithErrorCode(ErrorType.InvalidPhoneFormatError461.ToString())
            .WithMessage(ErrorType.InvalidPhoneFormatError461.GetMessage());

        RuleFor(x => x.Role)
            .IsInEnum().WithErrorCode(ErrorType.InvalidUserRoleError462.ToString())
            .WithMessage(ErrorType.InvalidUserRoleError462.GetMessage());
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
            .NotEmpty().WithErrorCode(ErrorType.EmptyUserIdError463.ToString())
            .WithMessage(ErrorType.EmptyUserIdError463.GetMessage());

        When(x => x.ApartmentNumber.HasValue, () =>
        {
            RuleFor(x => x.ApartmentNumber)
                .GreaterThan(0).WithErrorCode(ErrorType.InvalidApartmentNumberError451.ToString())
                .WithMessage(ErrorType.InvalidApartmentNumberError451.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.LastName), () =>
        {
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithErrorCode(ErrorType.LastNameTooLongError453.ToString())
                .WithMessage(ErrorType.LastNameTooLongError453.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.FirstName), () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithErrorCode(ErrorType.FirstNameTooLongError455.ToString())
                .WithMessage(ErrorType.FirstNameTooLongError455.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.MiddleName), () =>
        {
            RuleFor(x => x.MiddleName)
                .MaximumLength(50).WithErrorCode(ErrorType.MiddleNameTooLongError456.ToString())
                .WithMessage(ErrorType.MiddleNameTooLongError456.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .MinimumLength(8).WithErrorCode(ErrorType.PasswordTooShortError458.ToString())
                .WithMessage(ErrorType.PasswordTooShortError458.GetMessage())
                .Matches(PasswordRegex).WithErrorCode(ErrorType.InvalidPasswordFormatError459.ToString())
                .WithMessage(ErrorType.InvalidPasswordFormatError459.GetMessage());
        });

        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(PhoneRegex).WithErrorCode(ErrorType.InvalidPhoneFormatError461.ToString())
                .WithMessage(ErrorType.InvalidPhoneFormatError461.GetMessage());
        });

        When(x => x.Role.HasValue, () =>
        {
            RuleFor(x => x.Role)
                .IsInEnum().WithErrorCode(ErrorType.InvalidUserRoleError462.ToString())
                .WithMessage(ErrorType.InvalidUserRoleError462.GetMessage());
        });
    }
} 