using CommonTestUtilities.Requests;
using Ldc.Application.UseCases.Users.Register;
using Ldc.Exception;
using FluentAssertions;

namespace Validators.Test.Users.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    public void Error_Name_Empty(string name)
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = name;
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e .ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    public void Error_Email_Empty(string email)
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = email;
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e .ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY));
    }
    
    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = "bob.com";
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e .ErrorMessage.Equals(ResourceErrorMessages.EMAIL_INVALID));
    }
    
    [Fact]
    public void Error_Password_Empty()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = string.Empty;
        
        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e .ErrorMessage.Equals(ResourceErrorMessages.INVALID_PASSWORD));
    }
}