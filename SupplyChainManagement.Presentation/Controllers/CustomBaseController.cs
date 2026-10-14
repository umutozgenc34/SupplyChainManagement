using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace SupplyChainManagement.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult(ServiceResult result) =>
        StatusCode((int)result.Status, result);
}
