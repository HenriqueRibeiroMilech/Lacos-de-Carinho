using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.GetByLink;

public class GetWeddingListByLinkUseCase : IGetWeddingListByLinkUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetWeddingListByLinkUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseWeddingListJson> Execute(string shareableLink, bool isAuthenticated = false)
    {
        var weddingList = await _weddingListReadOnlyRepository.GetByShareableLink(shareableLink);

        if (weddingList is null)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        long? currentUserId = null;
        if (isAuthenticated)
        {
            try
            {
                var user = await _loggedUser.Get();
                currentUserId = user.Id;
            }
            catch
            {
                // User not authenticated, ignore
            }
        }

        return new ResponseWeddingListJson
        {
            Id = weddingList.Id,
            Title = weddingList.Title,
            Message = weddingList.Message,
            EventDate = weddingList.EventDate,
            DeliveryInfo = weddingList.DeliveryInfo,
            ListType = (Communication.Enums.ListType)weddingList.ListType,
            IsOwner = currentUserId.HasValue && weddingList.OwnerId == currentUserId.Value,
            Items = weddingList.Items.Select(i => new ResponseGiftItemJson
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Category = (Communication.Enums.GiftCategory)i.Category,
                Status = (Communication.Enums.GiftItemStatus)i.Status,
                ReservedById = i.ReservedById,
                ReservedByName = i.ReservedBy?.Name,
                MyReservationId = (currentUserId.HasValue && i.ReservedById == currentUserId) ? i.Id : null
            }).ToList()
        };
    }
}
