using Ldc.Application.UseCases.Users.Login.DoLogin;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUserReadOnlyRepository _repository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    
    public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        _repository = repository;
        _passwordEncrypter = passwordEncrypter;
        _accessTokenGenerator = accessTokenGenerator;
    }
    
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _repository.GetUserByEmail(request.Email);
        if (user is null)
        {
            throw new InvalidLoginException();
        }
        
        var passwordMatch = _passwordEncrypter.Verify(request.Password, user.Password);
        if (passwordMatch == false)
        {
            throw new InvalidLoginException();
        }
        
        return new ResponseRegisteredUserJson()
        {
            Name = user.Name,
            Token = _accessTokenGenerator.Generate(user)
        };
    }
}