using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Api.Contracts;
using NadinShop.Api.Extensions;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Application.Features.Identity.Commands.Login;
using NadinShop.Application.Features.Identity.Commands.Register;
using NadinShop.Application.Features.Identity.Queries.GetCurrentUser;

namespace NadinShop.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RegisterCommand(request.Email, request.Phone, request.Password),
            cancellationToken);

        if (result.IsFailure)
            return this.ToProblem(result.Error);

        return StatusCode(StatusCodes.Status201Created, new { id = result.Value });
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure)
            return this.ToProblem(result.Error);

        return Ok(result.Value.Adapt<AuthResponse>(MappingConfig.Config));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUser>> GetCurrentUser(CancellationToken cancellationToken)
        => this.ToActionResult(
            await mediator.Send(new GetCurrentUserQuery(), cancellationToken));
}
