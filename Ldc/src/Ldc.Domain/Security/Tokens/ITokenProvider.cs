namespace Ldc.Domain.Security.Tokens;

public interface ITokenProvider
{
    string TokenOnRequest();
}