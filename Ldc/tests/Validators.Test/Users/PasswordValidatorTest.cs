using Ldc.Application.UseCases.Users;
using Ldc.Communication.Requests;
using Ldc.Exception;
using FluentAssertions;
using FluentValidation;

namespace Validators.Test.Users;

public class PasswordValidatorTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    [InlineData("aaaaa")]
    [InlineData("aaaaaaaa")]
    [InlineData("AAAAAAAA")]
    [InlineData("Aaaaaaaa")]
    public void Error_Name_Empty(string password)
    {
        var validator = new PasswordValidator<RequestRegisterUserJson>();
        
        var result = validator.IsValid(new ValidationContext<RequestRegisterUserJson>(new RequestRegisterUserJson()), password);
        
        result.Should().BeFalse();
    }
}