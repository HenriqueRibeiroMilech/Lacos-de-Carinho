using FluentValidation;
using Ldc.Communication.Requests;
using Ldc.Exception;

namespace Ldc.Application.UseCases.GiftItems.Add;

public class AddGiftItemValidator : AbstractValidator<RequestAddGiftItemJson>
{
    public AddGiftItemValidator()
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .WithMessage(ResourceErrorMessages.NAME_EMPTY);
    }
}
