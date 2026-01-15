using Ldc.Communication.Responses;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.RegenerateLink;

public class RegenerateLinkUseCase : IRegenerateLinkUseCase
{
    private readonly IWeddingListUpdateOnlyRepository _weddingListUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public RegenerateLinkUseCase(
        IWeddingListUpdateOnlyRepository weddingListUpdateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListUpdateOnlyRepository = weddingListUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseWeddingListShortJson> Execute(long id)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListUpdateOnlyRepository.GetById(id);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        weddingList.ShareableLink = Guid.NewGuid().ToString("N")[..16];

        _weddingListUpdateOnlyRepository.Update(weddingList);
        await _unitOfWork.Commit();

        return new ResponseWeddingListShortJson
        {
            Id = weddingList.Id,
            Title = weddingList.Title,
            ShareableLink = weddingList.ShareableLink
        };
    }
}
