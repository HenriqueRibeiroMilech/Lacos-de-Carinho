namespace Ldc.Communication.Requests;

/// <summary>
/// Request para processar pagamento direto (Checkout Transparente)
/// </summary>
public class RequestProcessDirectPaymentJson
{
    /// <summary>
    /// Tipo de pagamento: "new_account" para novo cadastro ou "upgrade" para upgrade de conta
    /// </summary>
    public string PaymentType { get; set; } = "new_account";
    
    /// <summary>
    /// Token gerado pelo SDK MercadoPago.js (para cartão)
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// ID do método de pagamento (ex: "visa", "master", "pix")
    /// </summary>
    public string PaymentMethodId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID do emissor do cartão
    /// </summary>
    public string IssuerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de parcelas
    /// </summary>
    public int Installments { get; set; } = 1;
    
    /// <summary>
    /// Email do pagador (usado pelo MP para anti-fraude)
    /// </summary>
    public string PayerEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Dados do usuário para novo cadastro (opcional se for upgrade)
    /// </summary>
    public string? Name { get; set; }
    public string? Password { get; set; }
}
