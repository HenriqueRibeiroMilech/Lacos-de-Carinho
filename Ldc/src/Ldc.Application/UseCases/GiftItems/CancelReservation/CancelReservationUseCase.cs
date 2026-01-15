using Ldc.Communication.Responses;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.GiftItems.CancelReservation;

public class CancelReservationUseCase : ICancelReservationUseCase
{
    private readonly IGiftItemUpdateOnlyRepository _giftItemUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public CancelReservationUseCase(
        IGiftItemUpdateOnlyRepository giftItemUpdateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _giftItemUpdateOnlyRepository = giftItemUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseGiftItemJson> Execute(long itemId)
    {
        var user = await _loggedUser.Get();
        var giftItem = await _giftItemUpdateOnlyRepository.GetById(itemId);

        if (giftItem is null)
        {
            throw new NotFoundException(ResourceErrorMessages.GIFT_ITEM_NOT_FOUND);
        }

        if (giftItem.ReservedById != user.Id)
        {
            throw new ErrorOnValidationException([ResourceErrorMessages.GIFT_ITEM_NOT_RESERVED_BY_USER]);
        }

        giftItem.Status = GiftItemStatus.Available;
        giftItem.ReservedById = null;

        _giftItemUpdateOnlyRepository.Update(giftItem);
        await _unitOfWork.Commit();

        return new ResponseGiftItemJson
        {
            Id = giftItem.Id,
            Name = giftItem.Name,
            Description = giftItem.Description,
            Category = (Communication.Enums.GiftCategory)giftItem.Category,
            Status = (Communication.Enums.GiftItemStatus)giftItem.Status,
            ReservedById = giftItem.ReservedById
        };
    }
}
