using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UserUpdateOnlyRepositoryBuilder
{
    public static IUserUpdateOnlyRepository Build(User user)
    {
        var mock = new Mock<IUserUpdateOnlyRepository>();

        mock.Setup(repository => repository.GetById(user.Id)).ReturnsAsync(user);
        
        return mock.Object;
    }
}
