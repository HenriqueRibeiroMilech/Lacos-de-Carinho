using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.GiftItems.Add;

public class AddGiftItemUseCase : IAddGiftItemUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IGiftItemWriteOnlyRepository _giftItemWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public AddGiftItemUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IGiftItemWriteOnlyRepository giftItemWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _giftItemWriteOnlyRepository = giftItemWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseGiftItemJson> Execute(long listId, RequestAddGiftItemJson request)
    {
        Validate(request);

        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(listId);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var giftItem = new GiftItem
        {
            Name = request.Name,
            Description = request.Description,
            Category = (GiftCategory)request.Category,
            Status = GiftItemStatus.Available,
            WeddingListId = listId
        };

        await _giftItemWriteOnlyRepository.Add(giftItem);
        await _unitOfWork.Commit();

        return new ResponseGiftItemJson
        {
            Id = giftItem.Id,
            Name = giftItem.Name,
            Description = giftItem.Description,
            Category = (Communication.Enums.GiftCategory)giftItem.Category,
            Status = (Communication.Enums.GiftItemStatus)giftItem.Status
        };
    }

    private static void Validate(RequestAddGiftItemJson request)
    {
        var validator = new AddGiftItemValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
