using Ldc.Domain.Entities;

namespace Ldc.Domain.Security.Tokens;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}