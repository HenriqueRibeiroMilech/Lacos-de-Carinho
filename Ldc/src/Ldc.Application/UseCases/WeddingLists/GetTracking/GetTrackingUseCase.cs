using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.GetTracking;

public class GetTrackingUseCase : IGetTrackingUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IGiftItemReadOnlyRepository _giftItemReadOnlyRepository;
    private readonly IRsvpReadOnlyRepository _rsvpReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetTrackingUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IGiftItemReadOnlyRepository giftItemReadOnlyRepository,
        IRsvpReadOnlyRepository rsvpReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _giftItemReadOnlyRepository = giftItemReadOnlyRepository;
        _rsvpReadOnlyRepository = rsvpReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseTrackingJson> Execute(long listId)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(listId);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var giftItems = await _giftItemReadOnlyRepository.GetByWeddingListId(listId);
        var rsvps = await _rsvpReadOnlyRepository.GetByWeddingListId(listId);

        return new ResponseTrackingJson
        {
            ListId = listId,
            Gifts = giftItems
                .Where(g => g.ReservedById.HasValue)
                .Select(g => new ResponseGiftTrackingJson
                {
                    GiftItemId = g.Id,
                    GiftName = g.Name,
                    ReservedById = g.ReservedById,
                    ReservedByName = g.ReservedBy?.Name
                }).ToList(),
            Rsvps = rsvps.Select(r => new ResponseRsvpJson
            {
                Id = r.Id,
                GuestId = r.GuestId,
                GuestName = r.Guest?.Name,
                Status = (Communication.Enums.RsvpStatus)r.Status,
                AdditionalGuests = r.AdditionalGuests
            }).ToList()
        };
    }
}
