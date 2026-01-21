using Microsoft.AspNetCore.SignalR;

namespace Ldc.Api.Hubs;

/// <summary>
/// Hub SignalR para notificações de pagamento em tempo real
/// </summary>
public class PaymentHub : Hub
{
    private readonly ILogger<PaymentHub> _logger;

    public PaymentHub(ILogger<PaymentHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Chamado quando um cliente se conecta e quer receber notificações de um pagamento específico
    /// </summary>
    public async Task JoinPaymentGroup(string preferenceId)
    {
        if (string.IsNullOrEmpty(preferenceId))
        {
            _logger.LogWarning("Tentativa de join sem preferenceId");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"payment_{preferenceId}");
        _logger.LogInformation("Client {ConnectionId} joined payment group {PreferenceId}", 
            Context.ConnectionId, preferenceId);
    }

    /// <summary>
    /// Chamado quando cliente sai do grupo
    /// </summary>
    public async Task LeavePaymentGroup(string preferenceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"payment_{preferenceId}");
        _logger.LogInformation("Client {ConnectionId} left payment group {PreferenceId}", 
            Context.ConnectionId, preferenceId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(System.Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
