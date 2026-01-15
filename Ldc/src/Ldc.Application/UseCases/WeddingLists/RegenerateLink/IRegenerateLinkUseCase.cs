using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.RegenerateLink;

public interface IRegenerateLinkUseCase
{
    Task<ResponseWeddingListShortJson> Execute(long id);
}
