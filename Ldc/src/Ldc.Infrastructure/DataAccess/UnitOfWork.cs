using Ldc.Domain.Repositories;

namespace Ldc.Infrastructure.DataAccess;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApiDbContext _dbContext;
    
    public UnitOfWork(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Commit() => await _dbContext.SaveChangesAsync();
}