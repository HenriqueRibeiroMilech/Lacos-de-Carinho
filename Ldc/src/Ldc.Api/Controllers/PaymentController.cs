using Ldc.Application.UseCases.Payment.CreatePreference;
using Ldc.Application.UseCases.Payment.GetStatus;
using Ldc.Application.UseCases.Payment.ProcessDirect;
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
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processa pagamento direto via Checkout Transparente
    /// Para novo cadastro: não requer autenticação
    /// Para upgrade: requer autenticação
    /// </summary>
    [HttpPost("process")]
    [ProducesResponseType(typeof(ResponseDirectPaymentJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessDirectPayment(
        [FromServices] IProcessDirectPaymentUseCase useCase,
        [FromBody] RequestProcessDirectPaymentJson request)
    {
        _logger.LogInformation("Processing direct payment for type: {PaymentType}, method: {Method}",
            request.PaymentType, request.PaymentMethodId);
        var response = await useCase.Execute(request);
        _logger.LogInformation("Direct payment processed: Status={Status}, PaymentId={PaymentId}",
            response.Status, response.PaymentId);
        return Ok(response);
    }

    /// <summary>
    /// Cria uma preferência de pagamento no Mercado Pago (Checkout Pro - fallback)
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
        _logger.LogInformation("Creating payment preference for type: {PaymentType}", request.PaymentType);
        var response = await useCase.Execute(request);
        _logger.LogInformation("Payment preference created: {PreferenceId}", response.PreferenceId);
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
        _logger.LogInformation("Creating upgrade payment preference");
        var request = new RequestCreatePaymentJson { PaymentType = "upgrade" };
        var response = await useCase.Execute(request);
        _logger.LogInformation("Upgrade preference created: {PreferenceId}", response.PreferenceId);
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
        _logger.LogInformation("Getting payment status for: {PreferenceId}", preferenceId);
        var response = await useCase.Execute(preferenceId);
        _logger.LogInformation("Payment status: {Status}", response.Status);
        return Ok(response);
    }

    /// <summary>
    /// Polling endpoint para verificar status de Pix (checa diretamente no Mercado Pago)
    /// </summary>
    [HttpGet("check-pix/{mpPaymentId}")]
    [ProducesResponseType(typeof(ResponseDirectPaymentJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckPixStatus(
        [FromServices] IProcessDirectPaymentUseCase useCase,
        [FromRoute] long mpPaymentId)
    {
        _logger.LogInformation("Checking Pix payment status for MP PaymentId: {PaymentId}", mpPaymentId);
        var response = await useCase.CheckPixStatus(mpPaymentId);
        _logger.LogInformation("Pix status check result: {Status}", response.Status);
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
        [FromQuery(Name = "data.id")] string? dataId,
        [FromQuery] string? action)
    {
        _logger.LogInformation(
            "Webhook received - Type: {Type}, DataId: {DataId}, Action: {Action}",
            type, dataId, action);

        if (type == "payment" && !string.IsNullOrEmpty(dataId))
        {
            _logger.LogInformation("Processing payment webhook for PaymentId: {PaymentId}", dataId);
            await useCase.Execute(dataId);
            _logger.LogInformation("Webhook processed successfully for PaymentId: {PaymentId}", dataId);
        }
        else
        {
            _logger.LogInformation("Webhook ignored - not a payment type or missing dataId");
        }

        return Ok();
    }
}
