namespace Ldc.Domain.Repositories.Rsvp;

public interface IRsvpUpdateOnlyRepository
{
    Task<Entities.Rsvp?> GetByWeddingListAndGuest(long weddingListId, long guestId);
    void Update(Entities.Rsvp rsvp);
}
