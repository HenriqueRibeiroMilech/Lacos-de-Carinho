using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Payment.GetStatus;

public interface IGetPaymentStatusUseCase
{
    /// <summary>
    /// Verifica o status de um pagamento e retorna o token se aprovado
    /// </summary>
    Task<ResponsePaymentStatusJson> Execute(string preferenceId);
}
