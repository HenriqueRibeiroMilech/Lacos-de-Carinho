using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.Users.ChangePassword;

public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson request);
}