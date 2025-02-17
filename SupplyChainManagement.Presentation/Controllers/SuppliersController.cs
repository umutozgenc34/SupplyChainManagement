using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Create;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Update;
using SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;
using SupplyChainManagement.Application.Features.Suppliers.Queries.GetById;

namespace SupplyChainManagement.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllSuppliersQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetSupplierByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteSupplierCommand(id));
        return result.Success ? Ok(result) : NotFound(result);
    }
}