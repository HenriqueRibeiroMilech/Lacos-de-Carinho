using AutoMapper;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using FluentValidation.Results;

namespace Ldc.Application.UseCases.Users.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _tokenGenerator;
    
    public RegisterUserUseCase(IMapper mapper, IPasswordEncrypter passwordEncrypter, IUserReadOnlyRepository userReadOnlyRepository, IUserWriteOnlyRepository userWriteOnlyRepository, IUnitOfWork unitOfWork, IAccessTokenGenerator tokenGenerator)
    {
        _mapper = mapper;
        _passwordEncrypter = passwordEncrypter;
        _userReadOnlyRepository = userReadOnlyRepository;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        
    }
    
    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);
        
        var user = _mapper.Map<Ldc.Domain.Entities.User>(request);
        user.Password = _passwordEncrypter.Encrypt(request.Password);
        user.UserIdentifier = Guid.NewGuid();
        
        await _userWriteOnlyRepository.Add(user);
        await _unitOfWork.Commit();
        
        return new ResponseRegisteredUserJson()
        {
            Name = user.Name,
            Token = _tokenGenerator.Generate(user)
        };
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var result = new RegisterUserValidator().Validate(request);
        var emailExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
        if (emailExist)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
        }
        if (!result.IsValid)
        {
            var errorsMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorsMessages);
        }
    }
}