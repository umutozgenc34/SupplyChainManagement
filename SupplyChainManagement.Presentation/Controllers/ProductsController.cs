using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Products.Commands.Create;
using SupplyChainManagement.Application.Features.Products.Commands.Delete;
using SupplyChainManagement.Application.Features.Products.Commands.Update;
using SupplyChainManagement.Application.Features.Products.Queries.GetAll;
using SupplyChainManagement.Application.Features.Products.Queries.GetById;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Presentation.Controllers;

public class ProductsController(IMediator mediator) : CustomBaseController
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : CreateActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProductsQuery(pageNumber, pageSize), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpPut]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreateActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return CreateActionResult(result);
    }
}
