using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Users.Upgrade;

public interface IUpgradeUserUseCase
{
    Task<ResponseUpgradeUserJson> Execute();
}
