using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.Password.ResetPassword;

public interface IResetPasswordUseCase
{
    Task Execute(ResetPasswordJson request);
}
