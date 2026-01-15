using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Services.LoggedUser;

namespace Ldc.Application.UseCases.Guest.GetDetails;

public class GetGuestDetailsUseCase : IGetGuestDetailsUseCase
{
    private readonly IRsvpReadOnlyRepository _rsvpReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetGuestDetailsUseCase(
        IRsvpReadOnlyRepository rsvpReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _rsvpReadOnlyRepository = rsvpReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseGuestDetailsJson> Execute()
    {
        var user = await _loggedUser.Get();
        var rsvps = await _rsvpReadOnlyRepository.GetByGuestId(user.Id);

        return new ResponseGuestDetailsJson
        {
            UserId = user.Id,
            Events = rsvps.Select(r => new ResponseGuestEventRsvpJson
            {
                Rsvp = new ResponseRsvpJson
                {
                    Id = r.Id,
                    GuestId = r.GuestId,
                    GuestName = user.Name,
                    Status = (Communication.Enums.RsvpStatus)r.Status,
                    AdditionalGuests = r.AdditionalGuests
                },
                WeddingList = new ResponseWeddingListShortJson
                {
                    Id = r.WeddingList.Id,
                    Title = r.WeddingList.Title,
                    ShareableLink = r.WeddingList.ShareableLink
                }
            }).ToList()
        };
    }
}
