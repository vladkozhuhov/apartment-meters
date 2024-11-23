using Application.Users.Commands;

namespace Application.Users.Validators;

// public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
// {
//     public LoginUserCommandValidator()
//     {
//         RuleFor(x => x.Username)
//             .NotEmpty().WithMessage("Username is required.")
//             .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
//
//         RuleFor(x => x.Password)
//             .NotEmpty().WithMessage("Password is required.")
//             .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
//     }
// }