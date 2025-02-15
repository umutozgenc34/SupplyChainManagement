using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Products.Commands.Create;
using SupplyChainManagement.Application.Features.Products.Commands.Delete;
using SupplyChainManagement.Application.Features.Products.Commands.Update;
using SupplyChainManagement.Application.Features.Products.Queries.GetAll;
using SupplyChainManagement.Application.Features.Products.Queries.GetById;

namespace SupplyChainManagement.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllProductsQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update( [FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? Ok(result) : NotFound(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id));
        return result.Success ? Ok(result) : NotFound(result);
    }
}
