using CommonTestUtilities.Requests;
using Ldc.Application.UseCases.Users.ChangePassword;
using Ldc.Application.UseCases.Users.Update;
using Ldc.Communication.Requests;
using Ldc.Exception;
using FluentAssertions;

namespace Validators.Test.Users.ChangePassword;

public class ChangePasswordValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();
        var request = RequestChangePasswordJsonBuilder.Build();
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    public void Error_NewPassword_Empty(string newPassword)
    {
        var validator = new ChangePasswordValidator();
        
        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = newPassword;
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e .ErrorMessage.Equals(ResourceErrorMessages.INVALID_PASSWORD));
    }
    
}