namespace Ldc.Domain.Services.Payment;

public interface IMercadoPagoService
{
    /// <summary>
    /// Cria uma preferência de pagamento no Mercado Pago (Checkout Pro - fallback)
    /// </summary>
    Task<(string PreferenceId, string CheckoutUrl)> CreatePreference(
        string title,
        string description,
        decimal amount,
        string externalReference,
        string payerEmail);

    /// <summary>
    /// Processa um pagamento direto via Checkout Transparente
    /// O token do cartão é gerado pelo SDK MercadoPago.js no frontend
    /// </summary>
    Task<MercadoPagoDirectPaymentResult> CreatePayment(
        string token,
        decimal amount,
        string description,
        string payerEmail,
        string externalReference,
        string paymentMethodId,
        string issuerId,
        int installments);

    /// <summary>
    /// Consulta o status de um pagamento no Mercado Pago
    /// </summary>
    Task<string> GetPaymentStatus(string paymentId);

    /// <summary>
    /// Obtém detalhes completos de um pagamento no Mercado Pago
    /// </summary>
    Task<MercadoPagoPaymentDetails> GetPaymentDetails(string paymentId);
}

/// <summary>
/// Resultado de um pagamento direto (Checkout Transparente)
/// </summary>
public record MercadoPagoDirectPaymentResult(
    long PaymentId,
    string Status,
    string StatusDetail,
    string? PixQrCode,
    string? PixQrCodeBase64,
    string? TicketUrl
);

/// <summary>
/// Detalhes de um pagamento do Mercado Pago
/// </summary>
public record MercadoPagoPaymentDetails(
    string Status,
    string ExternalReference,
    string? PayerEmail,
    decimal? TransactionAmount
);
