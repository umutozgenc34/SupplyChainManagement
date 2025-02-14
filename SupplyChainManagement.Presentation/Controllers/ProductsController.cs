using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplyChainManagement.Application.Features.Products.Create;

namespace SupplyChainManagement.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var productId = await mediator.Send(command);
        return Ok(productId);
    }
}
