using Microsoft.AspNetCore.Mvc;
using Shop.Application.Auth.Commands;
using Shop.Application.Auth.Handlers;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;

    public AuthController(RegisterHandler registerHandler, LoginHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        await _registerHandler.Handle(command, cancellationToken);
        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var token = await _loginHandler.Handle(command, cancellationToken);
        return Ok(new { token });
    }


}
