using Ldc.Domain.Entities;

namespace Ldc.Domain.Repositories.PasswordReset;

public interface IPasswordResetTokenRepository
{
    Task Add(PasswordResetToken token);
    Task<PasswordResetToken?> GetByToken(string token);
    Task<PasswordResetToken?> GetValidTokenByUserId(long userId);
    Task InvalidateAllUserTokens(long userId);
}
