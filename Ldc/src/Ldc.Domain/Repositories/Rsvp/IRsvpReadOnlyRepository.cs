namespace Ldc.Domain.Repositories.Rsvp;

public interface IRsvpReadOnlyRepository
{
    Task<Entities.Rsvp?> GetById(long id);
    Task<Entities.Rsvp?> GetByWeddingListAndGuest(long weddingListId, long guestId);
    Task<List<Entities.Rsvp>> GetByWeddingListId(long weddingListId);
    Task<List<Entities.Rsvp>> GetByGuestId(long guestId);
}
