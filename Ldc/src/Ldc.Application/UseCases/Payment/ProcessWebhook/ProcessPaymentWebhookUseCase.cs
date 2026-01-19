using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Services.Payment;
using Microsoft.Extensions.Logging;

namespace Ldc.Application.UseCases.Payment.ProcessWebhook;

public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserUpdateOnlyRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPaymentWebhookUseCase> _logger;

    public ProcessPaymentWebhookUseCase(
        IMercadoPagoService mercadoPagoService,
        IPaymentRepository paymentRepository,
        IUserUpdateOnlyRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPaymentWebhookUseCase> logger)
    {
        _mercadoPagoService = mercadoPagoService;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(string paymentId)
    {
        _logger.LogInformation("Processing webhook for payment ID: {PaymentId}", paymentId);

        // Obtém detalhes completos do pagamento no Mercado Pago
        var paymentDetails = await _mercadoPagoService.GetPaymentDetails(paymentId);

        if (string.IsNullOrEmpty(paymentDetails.ExternalReference))
        {
            _logger.LogWarning("Payment {PaymentId} has no external_reference, cannot process", paymentId);
            return;
        }

        // Busca pelo PreferenceId interno (que é o external_reference enviado ao MP)
        var payment = await _paymentRepository.GetByPreferenceId(paymentDetails.ExternalReference);
        
        if (payment == null)
        {
            _logger.LogWarning(
                "Payment not found for external_reference: {ExternalReference} (MP PaymentId: {PaymentId})", 
                paymentDetails.ExternalReference, paymentId);
            return;
        }

        _logger.LogInformation(
            "Found payment record ID {PaymentRecordId} for user {UserId}. Current status: {OldStatus}, New status: {NewStatus}",
            payment.Id, payment.UserId, payment.Status, paymentDetails.Status);

        // Atualiza o MercadoPagoPaymentId real (agora temos o ID do pagamento, não da preferência)
        payment.MercadoPagoPaymentId = paymentId;
        payment.Status = paymentDetails.Status;

        if (paymentDetails.Status == "approved")
        {
            payment.PaidAt = DateTime.UtcNow;

            // Fazer upgrade do usuário para ADMIN
            var user = await _userRepository.GetById(payment.UserId);
            if (user != null && user.Role != Roles.ADMIN)
            {
                _logger.LogInformation("Upgrading user {UserId} to ADMIN role", user.Id);
                user.Role = Roles.ADMIN;
                _userRepository.Update(user);
            }
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.Commit();

        _logger.LogInformation(
            "Successfully processed payment {PaymentId}. Status: {Status}, User upgraded: {Upgraded}",
            paymentId, paymentDetails.Status, paymentDetails.Status == "approved");
    }
}
