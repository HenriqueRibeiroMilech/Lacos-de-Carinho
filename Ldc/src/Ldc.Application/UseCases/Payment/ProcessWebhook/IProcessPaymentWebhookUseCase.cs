namespace Ldc.Application.UseCases.Payment.ProcessWebhook;

/// <summary>
/// Resultado do processamento do webhook de pagamento
/// </summary>
public class WebhookProcessingResult
{
    public bool PaymentFound { get; set; }
    public bool Approved { get; set; }
    public string? PreferenceId { get; set; }
    public string? UserName { get; set; }
    public string? Token { get; set; }
}

public interface IProcessPaymentWebhookUseCase
{
    Task<WebhookProcessingResult> Execute(string paymentId);
}
