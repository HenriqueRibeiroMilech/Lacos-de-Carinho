using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.GetById;

public interface IGetWeddingListByIdUseCase
{
    Task<ResponseWeddingListJson> Execute(long id);
}
