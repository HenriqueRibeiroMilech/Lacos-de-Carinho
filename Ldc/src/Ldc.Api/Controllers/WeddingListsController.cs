using System.Net.Mime;
using Ldc.Application.UseCases.GiftItems.Add;
using Ldc.Application.UseCases.GiftItems.Delete;
using Ldc.Application.UseCases.GiftItems.Update;
using Ldc.Application.UseCases.Rsvps.Reports.Pdf;
using Ldc.Application.UseCases.WeddingLists.Create;
using Ldc.Application.UseCases.WeddingLists.Delete;
using Ldc.Application.UseCases.WeddingLists.GetAll;
using Ldc.Application.UseCases.WeddingLists.GetById;
using Ldc.Application.UseCases.WeddingLists.GetTracking;
using Ldc.Application.UseCases.WeddingLists.RegenerateLink;
using Ldc.Application.UseCases.WeddingLists.Update;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.ADMIN)]
public class WeddingListsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseCreateWeddingListJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromServices] ICreateWeddingListUseCase useCase,
        [FromBody] RequestCreateWeddingListJson request)
    {
        var response = await useCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseWeddingListsJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromServices] IGetAllWeddingListsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseWeddingListJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetWeddingListByIdUseCase useCase,
        [FromRoute] long id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateWeddingListUseCase useCase,
        [FromRoute] long id,
        [FromBody] RequestUpdateWeddingListJson request)
    {
        await useCase.Execute(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteWeddingListUseCase useCase,
        [FromRoute] long id)
    {
        await useCase.Execute(id);
        return NoContent();
    }

    [HttpPost("{id}/regenerate-link")]
    [ProducesResponseType(typeof(ResponseWeddingListShortJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateLink(
        [FromServices] IRegenerateLinkUseCase useCase,
        [FromRoute] long id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpGet("{id}/tracking")]
    [ProducesResponseType(typeof(ResponseTrackingJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracking(
        [FromServices] IGetTrackingUseCase useCase,
        [FromRoute] long id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpPost("{listId}/items")]
    [ProducesResponseType(typeof(ResponseGiftItemJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        [FromServices] IAddGiftItemUseCase useCase,
        [FromRoute] long listId,
        [FromBody] RequestAddGiftItemJson request)
    {
        var response = await useCase.Execute(listId, request);
        return Created(string.Empty, response);
    }

    [HttpDelete("{listId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(
        [FromServices] IDeleteGiftItemUseCase useCase,
        [FromRoute] long listId,
        [FromRoute] long itemId)
    {
        await useCase.Execute(listId, itemId);
        return NoContent();
    }

    [HttpPut("{listId}/items/{itemId}")]
    [ProducesResponseType(typeof(ResponseGiftItemJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(
        [FromServices] IUpdateGiftItemUseCase useCase,
        [FromRoute] long listId,
        [FromRoute] long itemId,
        [FromBody] RequestUpdateGiftItemJson request)
    {
        var response = await useCase.Execute(listId, itemId, request);
        return Ok(response);
    }

    [HttpGet("{id}/guests/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGuestListPdf(
        [FromServices] IGenerateGuestListPdfUseCase useCase,
        [FromRoute] long id)
    {
        byte[] file = await useCase.Execute(id);
        if (file.Length > 0)
            return File(file, MediaTypeNames.Application.Pdf, "lista-convidados.pdf");
        return NoContent();
    }
}
