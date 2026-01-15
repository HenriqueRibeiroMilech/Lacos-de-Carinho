using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Rsvps.Upsert;

public interface IUpsertRsvpUseCase
{
    Task<ResponseRsvpJson> Execute(long weddingListId, RequestUpsertRsvpJson request);
}
