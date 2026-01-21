namespace Ldc.Application.UseCases.Payment.ProcessWebhook;

public interface IProcessPaymentWebhookUseCase
{
    /// <summary>
    /// Processa notificação de webhook do Mercado Pago
    /// </summary>
    /// <param name="paymentId">ID do pagamento no Mercado Pago</param>
    Task Execute(string paymentId);
}
