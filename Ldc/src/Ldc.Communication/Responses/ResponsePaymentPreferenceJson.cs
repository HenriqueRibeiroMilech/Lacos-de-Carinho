namespace Ldc.Communication.Responses;

public class ResponsePaymentPreferenceJson
{
    /// <summary>
    /// URL do checkout do Mercado Pago para redirecionar o usuário
    /// </summary>
    public string CheckoutUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// ID da preferência criada
    /// </summary>
    public string PreferenceId { get; set; } = string.Empty;
}
