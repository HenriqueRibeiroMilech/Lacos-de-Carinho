using Ldc.Communication.Requests;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FluentValidation.Results;

namespace Ldc.Application.UseCases.Users.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public ChangePasswordUseCase(ILoggedUser loggedUser, IPasswordEncrypter passwordEncrypter, IUserUpdateOnlyRepository repository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _passwordEncrypter = passwordEncrypter;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();
        
        await Validate(request, loggedUser);
        
        var user = await _repository.GetById(loggedUser.Id);
        user.Password = _passwordEncrypter.Encrypt(request.NewPassword);
        
        _repository.Update(user);
        
        await _unitOfWork.Commit();
    }
    
    private async Task Validate(RequestChangePasswordJson request, User loggedUser)
    {
        var validator  = new ChangePasswordValidator();
        
        var result = validator.Validate(request);
        
        var passwordMatch = _passwordEncrypter.Verify(request.Password, loggedUser.Password);

        if (passwordMatch == false)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
        }

        if (result.IsValid == false)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }

    }
}