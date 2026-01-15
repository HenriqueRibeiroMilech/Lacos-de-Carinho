using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Guest.GetDetails;

public interface IGetGuestDetailsUseCase
{
    Task<ResponseGuestDetailsJson> Execute();
}
