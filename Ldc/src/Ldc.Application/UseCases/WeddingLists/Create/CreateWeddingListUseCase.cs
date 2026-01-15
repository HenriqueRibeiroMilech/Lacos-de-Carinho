using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.WeddingLists.Create;

public class CreateWeddingListUseCase : ICreateWeddingListUseCase
{
    private readonly IWeddingListWriteOnlyRepository _weddingListWriteOnlyRepository;
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public CreateWeddingListUseCase(
        IWeddingListWriteOnlyRepository weddingListWriteOnlyRepository,
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListWriteOnlyRepository = weddingListWriteOnlyRepository;
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseCreateWeddingListJson> Execute(RequestCreateWeddingListJson request)
    {
        Validate(request);

        var user = await _loggedUser.Get();

        // Check if user already has a list of this type
        var listTypeExists = await _weddingListReadOnlyRepository.ExistsByOwnerAndType(user.Id, (Domain.Enums.ListType)request.ListType);
        if (listTypeExists)
        {
            var listTypeName = request.ListType == Communication.Enums.ListType.Wedding ? "casamento" : "chá de panela";
            throw new ErrorOnValidationException(new List<string> { $"Você já possui uma lista de {listTypeName} ativa." });
        }

        var weddingList = new WeddingList
        {
            Title = request.Title,
            Message = request.Message,
            EventDate = request.EventDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            DeliveryInfo = request.DeliveryInfo,
            ListType = (Domain.Enums.ListType)request.ListType,
            ShareableLink = GenerateShareableLink(),
            OwnerId = user.Id
        };

        await _weddingListWriteOnlyRepository.Add(weddingList);
        await _unitOfWork.Commit();

        return new ResponseCreateWeddingListJson
        {
            Id = weddingList.Id,
            ShareableLink = weddingList.ShareableLink
        };
    }

    private static void Validate(RequestCreateWeddingListJson request)
    {
        var validator = new CreateWeddingListValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private static string GenerateShareableLink()
    {
        return Guid.NewGuid().ToString("N")[..16];
    }
}
