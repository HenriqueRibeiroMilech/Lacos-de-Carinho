using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Services.Payment;

namespace Ldc.Application.UseCases.Payment.ProcessWebhook;

public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserUpdateOnlyRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentWebhookUseCase(
        IMercadoPagoService mercadoPagoService,
        IPaymentRepository paymentRepository,
        IUserUpdateOnlyRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _mercadoPagoService = mercadoPagoService;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(string paymentId)
    {
        // Consulta status do pagamento no Mercado Pago
        var status = await _mercadoPagoService.GetPaymentStatus(paymentId);

        // Busca o pagamento pelo ID do MP (armazenado em MercadoPagoPaymentId)
        var payment = await _paymentRepository.GetByMercadoPagoPaymentId(paymentId);
        
        if (payment == null)
        {
            // Pode ser um pagamento que não reconhecemos
            return;
        }

        // Atualiza status
        payment.Status = status;

        if (status == "approved")
        {
            payment.PaidAt = DateTime.UtcNow;

            // Fazer upgrade do usuário para ADMIN
            var user = await _userRepository.GetById(payment.UserId);
            if (user.Role != Roles.ADMIN)
            {
                user.Role = Roles.ADMIN;
                _userRepository.Update(user);
            }
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.Commit();
    }
}
