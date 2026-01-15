using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.GetAll;

public interface IGetAllWeddingListsUseCase
{
    Task<ResponseWeddingListsJson> Execute();
}
