using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Products.Commands.Create;
using SupplyChainManagement.Application.Features.Products.Commands.Delete;
using SupplyChainManagement.Application.Features.Products.Commands.Update;
using SupplyChainManagement.Application.Features.Products.Queries.GetAll;
using SupplyChainManagement.Application.Features.Products.Queries.GetById;

namespace SupplyChainManagement.Presentation.Controllers;

public class ProductsController(IMediator mediator) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : CreateActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllProductsQuery());
        return CreateActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return CreateActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update( [FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return CreateActionResult(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id));
        return CreateActionResult(result);
    }
}
