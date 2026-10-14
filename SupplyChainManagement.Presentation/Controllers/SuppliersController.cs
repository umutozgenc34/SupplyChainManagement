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
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : CreateActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllSuppliersQuery(pageNumber, pageSize), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSupplierByIdQuery(id), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreateActionResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteSupplierCommand(id), cancellationToken);
        return CreateActionResult(result);
    }
}
