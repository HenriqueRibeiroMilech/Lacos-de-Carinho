using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.GiftItems.Add;

public interface IAddGiftItemUseCase
{
    Task<ResponseGiftItemJson> Execute(long listId, RequestAddGiftItemJson request);
}
