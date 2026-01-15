using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.WeddingLists.Update;

public interface IUpdateWeddingListUseCase
{
    Task Execute(long id, RequestUpdateWeddingListJson request);
}
