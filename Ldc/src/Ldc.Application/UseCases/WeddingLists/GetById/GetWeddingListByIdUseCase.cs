using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.GetById;

public class GetWeddingListByIdUseCase : IGetWeddingListByIdUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetWeddingListByIdUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseWeddingListJson> Execute(long id)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(id);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        return new ResponseWeddingListJson
        {
            Id = weddingList.Id,
            Title = weddingList.Title,
            Message = weddingList.Message,
            EventDate = weddingList.EventDate,
            DeliveryInfo = weddingList.DeliveryInfo,
            ShareableLink = weddingList.ShareableLink,
            ListType = (Communication.Enums.ListType)weddingList.ListType,
            Items = weddingList.Items.Select(i => new ResponseGiftItemJson
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Category = (Communication.Enums.GiftCategory)i.Category,
                Status = (Communication.Enums.GiftItemStatus)i.Status,
                ReservedById = i.ReservedById,
                ReservedByName = i.ReservedBy?.Name
            }).ToList(),
            Rsvps = weddingList.Rsvps.Select(r => new ResponseRsvpJson
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
