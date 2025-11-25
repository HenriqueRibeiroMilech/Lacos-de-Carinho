using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Users.Profile;

public interface IGetUserProfileUseCase
{
    Task<ResponseUserProfileJson> Execute();
}