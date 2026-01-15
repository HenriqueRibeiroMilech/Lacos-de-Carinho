using Ldc.Application.UseCases.Payment.CreatePreference;
using Ldc.Application.UseCases.Payment.GetStatus;
using Ldc.Application.UseCases.Payment.ProcessWebhook;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    /// <summary>
    /// Cria uma preferência de pagamento no Mercado Pago
    /// Para novo cadastro: não requer autenticação
    /// Para upgrade: requer autenticação
    /// </summary>
    [HttpPost("create-preference")]
    [ProducesResponseType(typeof(ResponsePaymentPreferenceJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePreference(
        [FromServices] ICreatePaymentPreferenceUseCase useCase,
        [FromBody] RequestCreatePaymentJson request)
    {
        var response = await useCase.Execute(request);
        return Ok(response);
    }

    /// <summary>
    /// Cria preferência de pagamento para upgrade (requer autenticação)
    /// </summary>
    [HttpPost("create-upgrade-preference")]
    [Authorize]
    [ProducesResponseType(typeof(ResponsePaymentPreferenceJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUpgradePreference(
        [FromServices] ICreatePaymentPreferenceUseCase useCase)
    {
        var request = new RequestCreatePaymentJson { PaymentType = "upgrade" };
        var response = await useCase.Execute(request);
        return Ok(response);
    }

    /// <summary>
    /// Consulta o status de um pagamento
    /// </summary>
    [HttpGet("status/{preferenceId}")]
    [ProducesResponseType(typeof(ResponsePaymentStatusJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(
        [FromServices] IGetPaymentStatusUseCase useCase,
        [FromRoute] string preferenceId)
    {
        var response = await useCase.Execute(preferenceId);
        return Ok(response);
    }

    /// <summary>
    /// Webhook para receber notificações do Mercado Pago
    /// </summary>
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Webhook(
        [FromServices] IProcessPaymentWebhookUseCase useCase,
        [FromQuery] string? type,
        [FromQuery(Name = "data.id")] string? dataId)
    {
        // Mercado Pago envia notificações com type=payment e data.id=paymentId
        if (type == "payment" && !string.IsNullOrEmpty(dataId))
        {
            await useCase.Execute(dataId);
        }

        // Sempre retorna 200 para o Mercado Pago não reenviar
        return Ok();
    }
}
