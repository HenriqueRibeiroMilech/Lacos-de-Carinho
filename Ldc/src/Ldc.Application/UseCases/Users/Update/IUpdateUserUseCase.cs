using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.Users.Update;

public interface IUpdateUserUseCase
{
    Task Execute(RequestUpdateUserJson request);
}