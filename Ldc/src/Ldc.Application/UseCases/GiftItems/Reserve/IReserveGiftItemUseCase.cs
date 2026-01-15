using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.GiftItems.Reserve;

public interface IReserveGiftItemUseCase
{
    Task<ResponseGiftItemJson> Execute(long itemId);
}
