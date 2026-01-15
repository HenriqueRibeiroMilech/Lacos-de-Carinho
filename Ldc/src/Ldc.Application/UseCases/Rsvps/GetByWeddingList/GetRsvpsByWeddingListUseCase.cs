using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Rsvps.GetByWeddingList;

public class GetRsvpsByWeddingListUseCase : IGetRsvpsByWeddingListUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IRsvpReadOnlyRepository _rsvpReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetRsvpsByWeddingListUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IRsvpReadOnlyRepository rsvpReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _rsvpReadOnlyRepository = rsvpReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<List<ResponseRsvpJson>> Execute(long weddingListId)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(weddingListId);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var rsvps = await _rsvpReadOnlyRepository.GetByWeddingListId(weddingListId);

        return rsvps.Select(r => new ResponseRsvpJson
        {
            Id = r.Id,
            GuestId = r.GuestId,
            GuestName = r.Guest?.Name,
            Status = (Communication.Enums.RsvpStatus)r.Status,
            AdditionalGuests = r.AdditionalGuests
        }).ToList();
    }
}
