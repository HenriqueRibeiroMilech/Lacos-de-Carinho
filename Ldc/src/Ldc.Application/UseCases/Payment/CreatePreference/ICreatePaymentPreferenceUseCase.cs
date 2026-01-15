using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Payment.CreatePreference;

public interface ICreatePaymentPreferenceUseCase
{
    /// <summary>
    /// Cria uma preferência de pagamento para novo cadastro ou upgrade
    /// </summary>
    Task<ResponsePaymentPreferenceJson> Execute(RequestCreatePaymentJson request);
}
