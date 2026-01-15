namespace Ldc.Communication.Requests;

public class RequestCreatePaymentJson
{
    /// <summary>
    /// Tipo de pagamento: "new_account" para novo cadastro ou "upgrade" para upgrade de conta
    /// </summary>
    public string PaymentType { get; set; } = "new_account";
    
    /// <summary>
    /// Dados do usuário para novo cadastro (opcional se for upgrade)
    /// </summary>
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
