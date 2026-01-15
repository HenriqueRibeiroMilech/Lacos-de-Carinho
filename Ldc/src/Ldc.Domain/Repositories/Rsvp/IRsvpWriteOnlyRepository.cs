namespace Ldc.Domain.Repositories.Rsvp;

public interface IRsvpWriteOnlyRepository
{
    Task Add(Entities.Rsvp rsvp);
}
