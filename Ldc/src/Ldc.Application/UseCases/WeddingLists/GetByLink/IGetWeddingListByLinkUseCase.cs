using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.WeddingLists.GetByLink;

public interface IGetWeddingListByLinkUseCase
{
    Task<ResponseWeddingListJson> Execute(string shareableLink, bool isAuthenticated = false);
}
