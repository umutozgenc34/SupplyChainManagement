using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Orders.Commands.ChangeStatus;
using SupplyChainManagement.Application.Features.Orders.Commands.Create;
using SupplyChainManagement.Application.Features.Orders.Commands.Delete;
using SupplyChainManagement.Application.Features.Orders.Commands.Update;
using SupplyChainManagement.Application.Features.Orders.Queries.GetAll;
using SupplyChainManagement.Application.Features.Orders.Queries.GetById;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Presentation.Controllers;

public class OrdersController(IMediator mediator) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : CreateActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreateActionResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { Id = id }, cancellationToken);
        return CreateActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteOrderCommand(id), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize), cancellationToken);
        return CreateActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return CreateActionResult(result);
    }
}
