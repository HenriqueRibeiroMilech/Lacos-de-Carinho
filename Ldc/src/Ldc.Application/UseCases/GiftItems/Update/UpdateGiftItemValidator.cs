using FluentValidation;
using Ldc.Communication.Requests;
using Ldc.Exception;

namespace Ldc.Application.UseCases.GiftItems.Update;

public class UpdateGiftItemValidator : AbstractValidator<RequestUpdateGiftItemJson>
{
    public UpdateGiftItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ResourceErrorMessages.NAME_EMPTY);
    }
}
