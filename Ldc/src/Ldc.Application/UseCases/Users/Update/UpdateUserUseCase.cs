using Ldc.Communication.Requests;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using FluentValidation.Results;

namespace Ldc.Application.UseCases.Users.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateUserUseCase(ILoggedUser loggedUser, IUserUpdateOnlyRepository repository, IUserReadOnlyRepository userReadOnlyRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();
        
        await Validate (request, loggedUser.Email);
        
        var user = await _repository.GetById(loggedUser.Id);
        
        user.Name = request.Name;
        user.Email = request.Email;
        
        _repository.Update(user);
        
        await _unitOfWork.Commit();
    }
    
    private async Task Validate(RequestUpdateUserJson request, string currentEmail)
    {
        var validator = new UpdateUserValidator();
        var result = validator.Validate(request);
        
        if (currentEmail.Equals(request.Email) == false)
        {
            var userExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (userExist)
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
        }
        if (!result.IsValid)
        {
            var errorsMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorsMessages);
        }
    }
}