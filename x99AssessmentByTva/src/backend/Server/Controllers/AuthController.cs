using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using x99AssessmentByTva.Application.Auth.Commands.Login;
using x99AssessmentByTva.Application.Auth.Commands.Register;

namespace x99AssessmentByTva.Server.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await sender.Send(command));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await sender.Send(command);
        return Created($"/api/auth/{result.UserId}", result);
    }
}
