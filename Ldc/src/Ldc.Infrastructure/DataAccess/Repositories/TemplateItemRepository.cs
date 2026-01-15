using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.TemplateItem;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class TemplateItemRepository : ITemplateItemReadOnlyRepository
{
    private readonly ApiDbContext _dbContext;

    public TemplateItemRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TemplateGiftItem>> GetAllWithCategories()
    {
        return await _dbContext.TemplateGiftItems
            .Include(t => t.Category)
            .AsNoTracking()
            .ToListAsync();
    }
}
