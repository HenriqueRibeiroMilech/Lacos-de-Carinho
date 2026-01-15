namespace Ldc.Domain.Services.Payment;

public interface IMercadoPagoService
{
    /// <summary>
    /// Cria uma preferência de pagamento no Mercado Pago
    /// </summary>
    /// <param name="title">Título do produto</param>
    /// <param name="description">Descrição</param>
    /// <param name="amount">Valor</param>
    /// <param name="externalReference">Referência externa (PreferenceId interno)</param>
    /// <param name="payerEmail">Email do pagador</param>
    /// <returns>Tupla com (PreferenceId, CheckoutUrl)</returns>
    Task<(string PreferenceId, string CheckoutUrl)> CreatePreference(
        string title,
        string description,
        decimal amount,
        string externalReference,
        string payerEmail);

    /// <summary>
    /// Consulta o status de um pagamento no Mercado Pago
    /// </summary>
    /// <param name="paymentId">ID do pagamento no MP</param>
    /// <returns>Status do pagamento</returns>
    Task<string> GetPaymentStatus(string paymentId);
}
