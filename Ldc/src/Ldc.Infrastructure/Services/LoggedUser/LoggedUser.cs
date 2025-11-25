using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ldc.Domain.Entities;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
    private readonly ApiDbContext _dbContext;
    private readonly ITokenProvider _tokenProvider;
    
    public LoggedUser (ApiDbContext dbContext, ITokenProvider tokenProvider)
    {
        _dbContext = dbContext;
        _tokenProvider = tokenProvider;
    }
    
    public async Task<User> Get()
    {
        string token = _tokenProvider.TokenOnRequest();

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        var identifier = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;

        return await _dbContext.Users.AsNoTracking().FirstAsync(user => user.UserIdentifier == Guid.Parse(identifier));
    }
}