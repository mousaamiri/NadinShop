using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Api.Contracts;
using NadinShop.Api.Extensions;
using NadinShop.Application.Features.Products.Commands.Create;
using NadinShop.Application.Features.Products.Commands.Delete;
using NadinShop.Application.Features.Products.Commands.Update;
using NadinShop.Application.Features.Products.Dtos;
using NadinShop.Application.Features.Products.Queries.Get;
using NadinShop.Application.Features.Products.Queries.GetById;
using NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;

namespace NadinShop.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
        => this.ToActionResult(await mediator.Send(new GetProductsQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
        => this.ToActionResult(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));

    [HttpGet("by-manufacture-email")]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetByManufactureEmail(
        [FromQuery] string manufactureEmail,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await mediator.Send(
                new GetProductsByManufactureEmailQuery(manufactureEmail),
                cancellationToken));

    [HttpPost]
    [Authorize]
    [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateProductCommand(request.Name, request.ProduceDate),
            cancellationToken);

        if (result.IsFailure)
            return this.ToActionResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType<UpdateProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateProductResponse>> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await mediator.Send(
                new UpdateProductCommand(id, request.Name, request.IsAvailable),
                cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => this.ToActionResult(
            await mediator.Send(new DeleteProductCommand(id), cancellationToken));
}
