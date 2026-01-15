using Ldc.Application.UseCases.TemplateItems.GetAll;
using Ldc.Communication.Responses;
using Ldc.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.ADMIN)]
public class TemplateItemsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ResponseTemplateItemsJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromServices] IGetAllTemplateItemsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }
}
