using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var token = await _mediator.Send(command);
        if (token == null) return Unauthorized();
        return Ok(token);
    }
}