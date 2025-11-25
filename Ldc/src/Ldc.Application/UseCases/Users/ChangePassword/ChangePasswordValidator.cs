using Ldc.Communication.Requests;
using FluentValidation;

namespace Ldc.Application.UseCases.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(user => user.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
    }
}