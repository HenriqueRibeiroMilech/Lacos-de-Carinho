using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.GiftItems.CancelReservation;

public interface ICancelReservationUseCase
{
    Task<ResponseGiftItemJson> Execute(long itemId);
}
