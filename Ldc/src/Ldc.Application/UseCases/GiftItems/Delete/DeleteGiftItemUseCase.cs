using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.GiftItems.Delete;

public class DeleteGiftItemUseCase : IDeleteGiftItemUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IGiftItemReadOnlyRepository _giftItemReadOnlyRepository;
    private readonly IGiftItemWriteOnlyRepository _giftItemWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteGiftItemUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IGiftItemReadOnlyRepository giftItemReadOnlyRepository,
        IGiftItemWriteOnlyRepository giftItemWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _giftItemReadOnlyRepository = giftItemReadOnlyRepository;
        _giftItemWriteOnlyRepository = giftItemWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task Execute(long listId, long itemId)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(listId);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var giftItem = await _giftItemReadOnlyRepository.GetById(itemId);

        if (giftItem is null || giftItem.WeddingListId != listId)
        {
            throw new NotFoundException(ResourceErrorMessages.GIFT_ITEM_NOT_FOUND);
        }

        await _giftItemWriteOnlyRepository.Delete(giftItem);
        await _unitOfWork.Commit();
    }
}
