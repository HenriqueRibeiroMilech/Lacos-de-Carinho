using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.Create;

public interface ICreateWeddingListUseCase
{
    Task<ResponseCreateWeddingListJson> Execute(RequestCreateWeddingListJson request);
}
