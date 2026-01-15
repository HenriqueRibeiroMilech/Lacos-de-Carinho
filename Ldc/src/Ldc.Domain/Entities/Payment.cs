namespace Ldc.Domain.Entities;

public class Payment
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    
    /// <summary>
    /// ID da preferência criada no Mercado Pago
    /// </summary>
    public string PreferenceId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID do pagamento no Mercado Pago (preenchido após confirmação)
    /// </summary>
    public string? MercadoPagoPaymentId { get; set; }
    
    /// <summary>
    /// Status do pagamento: pending, approved, rejected, cancelled
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// Valor do pagamento
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Descrição do produto/serviço
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de pagamento: new_account (novo cadastro) ou upgrade (convidado -> casal)
    /// </summary>
    public string PaymentType { get; set; } = "new_account";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
