using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.GetTracking;

public interface IGetTrackingUseCase
{
    Task<ResponseTrackingJson> Execute(long listId);
}
