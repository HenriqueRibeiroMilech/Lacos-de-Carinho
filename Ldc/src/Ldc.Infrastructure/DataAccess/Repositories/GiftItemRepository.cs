using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.GiftItem;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class GiftItemRepository : IGiftItemReadOnlyRepository, IGiftItemWriteOnlyRepository, IGiftItemUpdateOnlyRepository
{
    private readonly ApiDbContext _dbContext;

    public GiftItemRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GiftItem?> GetById(long id)
    {
        return await _dbContext.GiftItems
            .Include(g => g.ReservedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<GiftItem>> GetByWeddingListId(long weddingListId)
    {
        return await _dbContext.GiftItems
            .Include(g => g.ReservedBy)
            .AsNoTracking()
            .Where(g => g.WeddingListId == weddingListId)
            .ToListAsync();
    }

    public async Task Add(GiftItem giftItem)
    {
        await _dbContext.GiftItems.AddAsync(giftItem);
    }

    public Task Delete(GiftItem giftItem)
    {
        _dbContext.GiftItems.Remove(giftItem);
        return Task.CompletedTask;
    }

    async Task<GiftItem?> IGiftItemUpdateOnlyRepository.GetById(long id)
    {
        return await _dbContext.GiftItems.FirstOrDefaultAsync(g => g.Id == id);
    }

    public void Update(GiftItem giftItem)
    {
        _dbContext.GiftItems.Update(giftItem);
    }
}
