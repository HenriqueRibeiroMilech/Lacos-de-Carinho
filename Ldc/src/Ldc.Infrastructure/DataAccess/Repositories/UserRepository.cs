using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly ApiDbContext _dbcontext;
    
    public UserRepository(ApiDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await _dbcontext.Users.AnyAsync(user => user.Email.Equals(email));
    }

    public async Task<User> GetById(long id)
    {
        return await _dbcontext.Users.FirstAsync(user => user.Id == id);
    }

    public void Update(User user)
    {
        _dbcontext.Users.Update(user);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbcontext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email.Equals(email));
    }

    public async Task Add(User user)
    {
        await _dbcontext.Users.AddAsync(user);
    }

    public async Task Delete(User user)
    {
        var userToRemove = await _dbcontext.Users.FindAsync(user.Id);
        _dbcontext.Users.Remove(userToRemove!);
    }
}