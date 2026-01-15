using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.Delete;

public class DeleteWeddingListUseCase : IDeleteWeddingListUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IWeddingListWriteOnlyRepository _weddingListWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteWeddingListUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IWeddingListWriteOnlyRepository weddingListWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _weddingListWriteOnlyRepository = weddingListWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task Execute(long id)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListReadOnlyRepository.GetById(id);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        await _weddingListWriteOnlyRepository.Delete(id);
        await _unitOfWork.Commit();
    }
}
