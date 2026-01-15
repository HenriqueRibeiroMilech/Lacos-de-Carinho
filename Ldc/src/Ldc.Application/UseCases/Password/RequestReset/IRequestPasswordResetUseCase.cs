using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.Password.RequestReset;

public interface IRequestPasswordResetUseCase
{
    Task Execute(RequestPasswordResetJson request, string baseUrl);
}
