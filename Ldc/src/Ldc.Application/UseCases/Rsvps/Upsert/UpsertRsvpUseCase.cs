using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Rsvps.Upsert;

public class UpsertRsvpUseCase : IUpsertRsvpUseCase
{
    private readonly IWeddingListReadOnlyRepository _weddingListReadOnlyRepository;
    private readonly IRsvpReadOnlyRepository _rsvpReadOnlyRepository;
    private readonly IRsvpWriteOnlyRepository _rsvpWriteOnlyRepository;
    private readonly IRsvpUpdateOnlyRepository _rsvpUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public UpsertRsvpUseCase(
        IWeddingListReadOnlyRepository weddingListReadOnlyRepository,
        IRsvpReadOnlyRepository rsvpReadOnlyRepository,
        IRsvpWriteOnlyRepository rsvpWriteOnlyRepository,
        IRsvpUpdateOnlyRepository rsvpUpdateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _weddingListReadOnlyRepository = weddingListReadOnlyRepository;
        _rsvpReadOnlyRepository = rsvpReadOnlyRepository;
        _rsvpWriteOnlyRepository = rsvpWriteOnlyRepository;
        _rsvpUpdateOnlyRepository = rsvpUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseRsvpJson> Execute(long weddingListId, RequestUpsertRsvpJson request)
    {
        var user = await _loggedUser.Get();
        
        var weddingList = await _weddingListReadOnlyRepository.GetById(weddingListId);
        if (weddingList is null)
        {
            throw new NotFoundException(ResourceErrorMessages.WEDDING_LIST_NOT_FOUND);
        }

        var existingRsvp = await _rsvpUpdateOnlyRepository.GetByWeddingListAndGuest(weddingListId, user.Id);

        if (existingRsvp is not null)
        {
            existingRsvp.Status = (Domain.Enums.RsvpStatus)request.Status;
            existingRsvp.AdditionalGuests = request.AdditionalGuests;

            _rsvpUpdateOnlyRepository.Update(existingRsvp);
            await _unitOfWork.Commit();

            return new ResponseRsvpJson
            {
                Id = existingRsvp.Id,
                GuestId = existingRsvp.GuestId,
                GuestName = user.Name,
                Status = (Communication.Enums.RsvpStatus)existingRsvp.Status,
                AdditionalGuests = existingRsvp.AdditionalGuests
            };
        }

        var rsvp = new Rsvp
        {
            WeddingListId = weddingListId,
            GuestId = user.Id,
            Status = (Domain.Enums.RsvpStatus)request.Status,
            AdditionalGuests = request.AdditionalGuests
        };

        await _rsvpWriteOnlyRepository.Add(rsvp);
        await _unitOfWork.Commit();

        return new ResponseRsvpJson
        {
            Id = rsvp.Id,
            GuestId = rsvp.GuestId,
            GuestName = user.Name,
            Status = (Communication.Enums.RsvpStatus)rsvp.Status,
            AdditionalGuests = rsvp.AdditionalGuests
        };
    }
}
