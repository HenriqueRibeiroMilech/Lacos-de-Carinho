using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.Rsvp;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class RsvpRepository : IRsvpReadOnlyRepository, IRsvpWriteOnlyRepository, IRsvpUpdateOnlyRepository
{
    private readonly ApiDbContext _dbContext;

    public RsvpRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Rsvp?> GetById(long id)
    {
        return await _dbContext.Rsvps
            .Include(r => r.Guest)
            .Include(r => r.WeddingList)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rsvp?> GetByWeddingListAndGuest(long weddingListId, long guestId)
    {
        return await _dbContext.Rsvps
            .Include(r => r.Guest)
            .Include(r => r.WeddingList)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.WeddingListId == weddingListId && r.GuestId == guestId);
    }

    public async Task<List<Rsvp>> GetByWeddingListId(long weddingListId)
    {
        return await _dbContext.Rsvps
            .Include(r => r.Guest)
            .AsNoTracking()
            .Where(r => r.WeddingListId == weddingListId)
            .ToListAsync();
    }

    public async Task<List<Rsvp>> GetByGuestId(long guestId)
    {
        return await _dbContext.Rsvps
            .Include(r => r.WeddingList)
            .AsNoTracking()
            .Where(r => r.GuestId == guestId)
            .ToListAsync();
    }

    public async Task Add(Rsvp rsvp)
    {
        await _dbContext.Rsvps.AddAsync(rsvp);
    }

    async Task<Rsvp?> IRsvpUpdateOnlyRepository.GetByWeddingListAndGuest(long weddingListId, long guestId)
    {
        return await _dbContext.Rsvps.FirstOrDefaultAsync(r => r.WeddingListId == weddingListId && r.GuestId == guestId);
    }

    public void Update(Rsvp rsvp)
    {
        _dbContext.Rsvps.Update(rsvp);
    }
}
