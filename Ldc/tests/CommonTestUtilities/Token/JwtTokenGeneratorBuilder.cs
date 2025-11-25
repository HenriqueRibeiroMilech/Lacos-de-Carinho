using Ldc.Domain.Entities;
using Ldc.Domain.Security.Tokens;
using Moq;

namespace CommonTestUtilities.Token;

public class JwtTokenGeneratorBuilder
{
    public static IAccessTokenGenerator Build()
    {
        var mock = new Mock<IAccessTokenGenerator>();

        mock.Setup(accessTokenGenerator => accessTokenGenerator.Generate(It.IsAny<User>())).Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN0cmluZyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3NpZCI6IjQ2ZDYxYzg3LTM1YTctNDNhYi05Yjc2LTQ1MTExMjk2YWQ3MiIsIm5iZiI8MTc2MTA5OTM3MywiZXhwIhoxNzZxLTQ5MzczLCJpYXQiOjE3NjEwODkzNzN9.lHMViHut3Unmdu07TYJmdebR8tXHbXdaiJEVhoXhtZo");
        
        return mock.Object;
    }
}