using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Ldc.Application.UseCases.Users.ChangePassword;
using Ldc.Domain.Entities;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using FluentAssertions;

namespace UseCases.Test.Users.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
    
        var useCase = CreateUseCase(user, request.Password);
        
        var act = async () => await useCase.Execute(request);
        
        await act.Should().NotThrowAsync();
    } 
    
    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        var user = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = string.Empty;
        
        var useCase = CreateUseCase(user, request.Password);
        
        var act = async () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

        result.Where(ex =>
            ex.GetErrors().Count == 1 && ex.GetErrors().Contains(ResourceErrorMessages.INVALID_PASSWORD));
    }         
    
    [Fact]
    public async Task Error_CurrentPassword_Different()
    {
        var user = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        
        var useCase = CreateUseCase(user);
        
        var act = async () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

        result.Where(ex =>
            ex.GetErrors().Count == 1 && ex.GetErrors().Contains(ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
    }     

    
    private ChangePasswordUseCase CreateUseCase(User user, string? password = null)
    {
        var userUpdateRepository = UserUpdateOnlyRepositoryBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var passwordEncripter = new PasswordEncrypterBuilder().Verify(password).Build();
        
        return new ChangePasswordUseCase(loggedUser, passwordEncripter, userUpdateRepository, unitOfWork);
    }
}