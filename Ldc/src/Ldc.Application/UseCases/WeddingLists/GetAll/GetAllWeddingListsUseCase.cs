using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;

namespace Ldc.Application.UseCases.WeddingLists.GetAll;

public class GetAllWeddingListsUseCase : IGetAllWeddingListsUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetAllWeddingListsUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseWeddingListsJson> Execute()
    {
        var user = await _loggedUser.Get();
        var weddingLists = await _weddingListReadOnlyRepository.GetAllByOwnerIdWithDetails(user.Id);

        return new ResponseWeddingListsJson
        {
            Lists = weddingLists.Select(w => new ResponseWeddingListShortJson
            {
                Id = w.Id,
                Title = w.Title,
                ShareableLink = w.ShareableLink,
                EventDate = w.EventDate.ToDateTime(TimeOnly.MinValue),
                Message = w.Message,
                ListType = (Communication.Enums.ListType)w.ListType,
                TotalItems = w.Items.Count,
                ReservedItems = w.Items.Count(i => i.Status == Domain.Enums.GiftItemStatus.Reserved),
                TotalRsvps = w.Rsvps.Count,
                ConfirmedRsvps = CalculateTotalConfirmed(w.Rsvps)
            }).ToList()
        };
    }

    private static int CalculateTotalConfirmed(ICollection<Domain.Entities.Rsvp> rsvps)
    {
        var confirmedRsvps = rsvps.Where(r => r.Status == Domain.Enums.RsvpStatus.Confirmed);
        
        // Conta confirmados + acompanhantes de cada confirmado
        var total = 0;
        foreach (var rsvp in confirmedRsvps)
        {
            total++; // O próprio confirmado
            
            // Conta acompanhantes (separados por vírgula)
            if (!string.IsNullOrWhiteSpace(rsvp.AdditionalGuests))
            {
                var guests = rsvp.AdditionalGuests.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g));
                total += guests.Count();
            }
        }
        
        return total;
    }
}
