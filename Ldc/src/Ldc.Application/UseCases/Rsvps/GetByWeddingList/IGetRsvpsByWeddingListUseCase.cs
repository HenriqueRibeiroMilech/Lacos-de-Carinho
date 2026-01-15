using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Rsvps.GetByWeddingList;

public interface IGetRsvpsByWeddingListUseCase
{
    Task<List<ResponseRsvpJson>> Execute(long weddingListId);
}
