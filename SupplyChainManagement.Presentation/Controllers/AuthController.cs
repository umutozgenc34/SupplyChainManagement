using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Auth.Commands.Login;
using SupplyChainManagement.Application.Features.Auth.Commands.Register;

namespace SupplyChainManagement.Presentation.Controllers;

[AllowAnonymous]
public class AuthController(IMediator mediator) : CustomBaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreateActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreateActionResult(result);
    }
}
