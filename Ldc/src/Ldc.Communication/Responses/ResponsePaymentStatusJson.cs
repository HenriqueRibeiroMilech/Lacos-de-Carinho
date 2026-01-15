namespace Ldc.Communication.Responses;

public class ResponsePaymentStatusJson
{
    /// <summary>
    /// Status do pagamento: pending, approved, rejected, cancelled
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Token JWT do usuário (apenas se aprovado)
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Mensagem informativa
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
