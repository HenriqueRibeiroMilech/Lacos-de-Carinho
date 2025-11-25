using Ldc.Domain.Entities;

namespace Ldc.Domain.Services.LoggedUser;

public interface ILoggedUser
{
    Task<User> Get();
}