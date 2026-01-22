using Ldc.Communication.Responses;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Repositories;

namespace Ldc.Application.UseCases.Payment.GetStatus;

public class GetPaymentStatusUseCase : IGetPaymentStatusUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserUpdateOnlyRepository _userRepository;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentStatusUseCase(
        IPaymentRepository paymentRepository,
        IUserUpdateOnlyRepository userRepository,
        IAccessTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponsePaymentStatusJson> Execute(string preferenceId)
    {
        var payment = await _paymentRepository.GetByPreferenceId(preferenceId);

        if (payment == null)
        {
            return new ResponsePaymentStatusJson
            {
                Status = "not_found",
                Message = "Pagamento não encontrado."
            };
        }

        // Busca o usuário para verificar a role
        var user = await _userRepository.GetById(payment.UserId);
        
        // FALLBACK: Se o usuário já é ADMIN (webhook já atualizou), considera aprovado
        // mesmo que payment.Status não tenha sido atualizado corretamente
        var effectiveStatus = payment.Status;
        if (user != null && user.Role == Roles.ADMIN && payment.Status != "approved")
        {
            // Corrige o status do pagamento para manter consistência
            payment.Status = "approved";
            payment.PaidAt ??= DateTime.UtcNow;
            _paymentRepository.Update(payment);
            await _unitOfWork.Commit();
            effectiveStatus = "approved";
        }

        var response = new ResponsePaymentStatusJson
        {
            Status = effectiveStatus
        };

        switch (effectiveStatus)
        {
            case "approved":
                // Garante que o usuário é ADMIN
                if (user.Role != Roles.ADMIN)
                {
                    user.Role = Roles.ADMIN;
                    _userRepository.Update(user);
                    await _unitOfWork.Commit();
                }
                
                response.Token = _tokenGenerator.Generate(user);
                response.Name = user.Name;
                response.Message = "Pagamento aprovado! Bem-vindo ao Laços de Carinho.";
                break;

            case "pending":
                response.Message = "Aguardando confirmação do pagamento...";
                break;

            case "rejected":
                response.Message = "Pagamento não aprovado. Tente novamente.";
                break;

            default:
                response.Message = "Status do pagamento: " + effectiveStatus;
                break;
        }

        return response;
    }
}
