using FluentValidation;
using Ldc.Communication.Requests;
using Ldc.Exception;

namespace Ldc.Application.UseCases.WeddingLists.Create;

public class CreateWeddingListValidator : AbstractValidator<RequestCreateWeddingListJson>
{
    public CreateWeddingListValidator()
    {
        RuleFor(w => w.Title)
            .NotEmpty()
            .WithMessage(ResourceErrorMessages.TITLE_REQUIRED);
    }
}
