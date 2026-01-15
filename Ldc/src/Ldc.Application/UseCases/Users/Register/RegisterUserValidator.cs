using Ldc.Communication.Requests;
using Ldc.Domain.Enums;
using Ldc.Exception;
using FluentValidation;

namespace Ldc.Application.UseCases.Users.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    private static readonly string[] ValidRoles = { Roles.ADMIN, Roles.USER };
    
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceErrorMessages.NAME_EMPTY);
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage(ResourceErrorMessages.EMAIL_EMPTY)
            .EmailAddress().When(user => string.IsNullOrWhiteSpace(user.Email) == false, ApplyConditionTo.CurrentValidator).WithMessage(ResourceErrorMessages.EMAIL_INVALID);
        RuleFor(user => user.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
        RuleFor(user => user.Role)
            .NotEmpty().WithMessage(ResourceErrorMessages.ROLE_REQUIRED)
            .Must(role => ValidRoles.Contains(role.ToLower())).WithMessage(ResourceErrorMessages.ROLE_INVALID);
    }
}