using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Create;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Update;
using SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;
using SupplyChainManagement.Application.Features.Suppliers.Queries.GetById;

namespace SupplyChainManagement.Presentation.Controllers;

public class SuppliersController(IMediator mediator) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await mediator.Send(command);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : CreateActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllSuppliersQuery());
        return CreateActionResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetSupplierByIdQuery(id));
        return CreateActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierCommand command)
    {
        var result = await mediator.Send(command);
        return CreateActionResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteSupplierCommand(id));
        return CreateActionResult(result);
    }
}