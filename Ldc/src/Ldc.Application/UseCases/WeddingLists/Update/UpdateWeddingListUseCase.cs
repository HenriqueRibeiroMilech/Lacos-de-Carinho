using Ldc.Communication.Requests;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.Update;

public class UpdateWeddingListUseCase : IUpdateWeddingListUseCase
{
    private readonly IWeddingListUpdateOnlyRepository _weddingListUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public UpdateWeddingListUseCase(
        IWeddingListUpdateOnlyRepository weddingListUpdateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListUpdateOnlyRepository = weddingListUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task Execute(long id, RequestUpdateWeddingListJson request)
    {
        var user = await _loggedUser.Get();
        var weddingList = await _weddingListUpdateOnlyRepository.GetById(id);

        if (weddingList is null || weddingList.OwnerId != user.Id)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
            weddingList.Title = request.Title;

        if (request.Message is not null)
            weddingList.Message = request.Message;

        if (request.EventDate.HasValue)
            weddingList.EventDate = request.EventDate.Value;

        if (request.DeliveryInfo is not null)
            weddingList.DeliveryInfo = request.DeliveryInfo;

        _weddingListUpdateOnlyRepository.Update(weddingList);
        await _unitOfWork.Commit();
    }
}
