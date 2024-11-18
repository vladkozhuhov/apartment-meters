using MediatR;

namespace Application.Users.Commands;

public class LoginUserCommand : IRequest<string>
{
    public string ApartmentNumber { get; set; }
    public string Password { get; set; }
}