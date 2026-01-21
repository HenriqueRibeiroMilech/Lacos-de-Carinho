using Ldc.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Ldc.Api.Services;

/// <summary>
/// Serviço para enviar notificações de pagamento via SignalR
/// </summary>
public interface IPaymentNotificationService
{
    Task NotifyPaymentApproved(string preferenceId, string userName, string token);
}

public class PaymentNotificationService : IPaymentNotificationService
{
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<PaymentNotificationService> _logger;

    public PaymentNotificationService(
        IHubContext<PaymentHub> hubContext,
        ILogger<PaymentNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyPaymentApproved(string preferenceId, string userName, string token)
    {
        _logger.LogInformation("Sending payment approval notification for preferenceId: {PreferenceId}", preferenceId);
        
        await _hubContext.Clients.Group($"payment_{preferenceId}")
            .SendAsync("PaymentApproved", new
            {
                PreferenceId = preferenceId,
                UserName = userName,
                Token = token,
                Message = "Pagamento aprovado com sucesso!"
            });
        
        _logger.LogInformation("Payment approval notification sent for preferenceId: {PreferenceId}", preferenceId);
    }
}
