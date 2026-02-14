using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Payment.ProcessDirect;

public interface IProcessDirectPaymentUseCase
{
    Task<ResponseDirectPaymentJson> Execute(Communication.Requests.RequestProcessDirectPaymentJson request);
    Task<ResponseDirectPaymentJson> CheckPixStatus(long mpPaymentId);
}
