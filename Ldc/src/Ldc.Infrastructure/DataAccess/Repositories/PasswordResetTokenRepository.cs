using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.PasswordReset;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ApiDbContext _dbContext;

    public PasswordResetTokenRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(PasswordResetToken token)
    {
        await _dbContext.PasswordResetTokens.AddAsync(token);
    }

    public async Task<PasswordResetToken?> GetByToken(string token)
    {
        return await _dbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<PasswordResetToken?> GetValidTokenByUserId(long userId)
    {
        return await _dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task InvalidateAllUserTokens(long userId)
    {
        var tokens = await _dbContext.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsUsed = true;
        }
    }
}
