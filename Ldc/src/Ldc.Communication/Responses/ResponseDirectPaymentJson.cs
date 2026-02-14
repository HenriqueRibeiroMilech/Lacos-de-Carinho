namespace Ldc.Communication.Responses;

/// <summary>
/// Resposta do processamento de pagamento direto (Checkout Transparente)
/// </summary>
public class ResponseDirectPaymentJson
{
    /// <summary>
    /// Status do pagamento: approved, pending, rejected, in_process
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Detalhe do status (ex: "accredited", "pending_waiting_payment")
    /// </summary>
    public string StatusDetail { get; set; } = string.Empty;
    
    /// <summary>
    /// Mensagem amigável para o usuário
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Token JWT do usuário (apenas se aprovado)
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// ID do pagamento no Mercado Pago
    /// </summary>
    public long PaymentId { get; set; }
    
    // ---- Dados de Pix (quando aplicável) ----
    
    /// <summary>
    /// Código Pix copia-e-cola
    /// </summary>
    public string? PixQrCode { get; set; }
    
    /// <summary>
    /// QR Code em base64 para exibir imagem
    /// </summary>
    public string? PixQrCodeBase64 { get; set; }
    
    /// <summary>
    /// URL do ticket de pagamento (Pix)
    /// </summary>
    public string? TicketUrl { get; set; }
}
