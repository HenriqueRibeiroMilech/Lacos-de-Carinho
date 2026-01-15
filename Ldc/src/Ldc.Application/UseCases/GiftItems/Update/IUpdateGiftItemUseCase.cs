using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.GiftItems.Update;

public interface IUpdateGiftItemUseCase
{
    Task<ResponseGiftItemJson> Execute(long listId, long itemId, RequestUpdateGiftItemJson request);
}
