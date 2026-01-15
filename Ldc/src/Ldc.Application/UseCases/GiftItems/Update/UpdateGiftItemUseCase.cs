using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.GiftItems.Update;

public class UpdateGiftItemUseCase : IUpdateGiftItemUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IGiftItemUpdateOnlyRepository _giftItemUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public UpdateGiftItemUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IGiftItemUpdateOnlyRepository giftItemUpdateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _giftItemUpdateOnlyRepository = giftItemUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseGiftItemJson> Execute(long listId, long itemId, RequestUpdateGiftItemJson request)
    {
        Validate(request);

        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(listId);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var giftItem = await _giftItemUpdateOnlyRepository.GetById(itemId);

        if (giftItem is null || giftItem.WeddingListId != listId)
        {
            throw new NotFoundException(ResourceErrorMessages.GIFT_ITEM_NOT_FOUND);
        }

        giftItem.Name = request.Name;
        giftItem.Description = request.Description;
        if (request.Category.HasValue)
        {
            giftItem.Category = (Domain.Enums.GiftCategory)request.Category.Value;
        }

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

    private static void Validate(RequestUpdateGiftItemJson request)
    {
        var validator = new UpdateGiftItemValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
