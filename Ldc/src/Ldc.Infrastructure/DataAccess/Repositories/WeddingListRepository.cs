using Ldc.Domain.Entities;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories.WeddingList;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class WeddingListRepository : IWeddingListReadOnlyRepository, IWeddingListWriteOnlyRepository, IWeddingListUpdateOnlyRepository
{
    private readonly ApiDbContext _dbContext;

    public WeddingListRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WeddingList?> GetById(long id)
    {
        return await _dbContext.WeddingLists
            .Include(w => w.Items)
                .ThenInclude(i => i.ReservedBy)
            .Include(w => w.Rsvps)
                .ThenInclude(r => r.Guest)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WeddingList?> GetByShareableLink(string shareableLink)
    {
        return await _dbContext.WeddingLists
            .Include(w => w.Items)
                .ThenInclude(i => i.ReservedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.ShareableLink == shareableLink);
    }

    public async Task<List<WeddingList>> GetAllByOwnerId(long ownerId)
    {
        return await _dbContext.WeddingLists
            .AsNoTracking()
            .Where(w => w.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<List<WeddingList>> GetAllByOwnerIdWithDetails(long ownerId)
    {
        return await _dbContext.WeddingLists
            .Include(w => w.Items)
            .Include(w => w.Rsvps)
            .AsNoTracking()
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.EventDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsById(long id)
    {
        return await _dbContext.WeddingLists.AnyAsync(w => w.Id == id);
    }

    public async Task<bool> ExistsByOwnerAndType(long ownerId, ListType listType)
    {
        return await _dbContext.WeddingLists.AnyAsync(w => w.OwnerId == ownerId && w.ListType == listType);
    }

    public async Task Add(WeddingList weddingList)
    {
        await _dbContext.WeddingLists.AddAsync(weddingList);
    }

    public async Task Delete(long id)
    {
        var weddingList = await _dbContext.WeddingLists.FindAsync(id);
        if (weddingList != null)
        {
            _dbContext.WeddingLists.Remove(weddingList);
        }
    }

    async Task<WeddingList?> IWeddingListUpdateOnlyRepository.GetById(long id)
    {
        return await _dbContext.WeddingLists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public void Update(WeddingList weddingList)
    {
        _dbContext.WeddingLists.Update(weddingList);
    }
}
