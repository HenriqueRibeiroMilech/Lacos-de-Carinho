using Ldc.Application.UseCases.GiftItems.CancelReservation;
using Ldc.Application.UseCases.GiftItems.Reserve;
using Ldc.Application.UseCases.Guest.GetDetails;
using Ldc.Application.UseCases.Rsvps.Upsert;
using Ldc.Application.UseCases.WeddingLists.GetByLink;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GuestController : ControllerBase
{
    [HttpGet("lists/{shareableLink}")]
    [ProducesResponseType(typeof(ResponseWeddingListJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListByLink(
        [FromServices] IGetWeddingListByLinkUseCase useCase,
        [FromRoute] string shareableLink)
    {
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var response = await useCase.Execute(shareableLink, isAuthenticated);
        return Ok(response);
    }

    [HttpPost("items/{itemId}/reserve")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseGiftItemJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReserveItem(
        [FromServices] IReserveGiftItemUseCase useCase,
        [FromRoute] long itemId)
    {
        var response = await useCase.Execute(itemId);
        return Ok(response);
    }

    [HttpDelete("items/{itemId}/reserve")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseGiftItemJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReservation(
        [FromServices] ICancelReservationUseCase useCase,
        [FromRoute] long itemId)
    {
        var response = await useCase.Execute(itemId);
        return Ok(response);
    }

    [HttpPost("lists/{weddingListId}/rsvp")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseRsvpJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpsertRsvp(
        [FromServices] IUpsertRsvpUseCase useCase,
        [FromRoute] long weddingListId,
        [FromBody] RequestUpsertRsvpJson request)
    {
        var response = await useCase.Execute(weddingListId, request);
        return Ok(response);
    }

    [HttpGet("me/details")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseGuestDetailsJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDetails([FromServices] IGetGuestDetailsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }
}
