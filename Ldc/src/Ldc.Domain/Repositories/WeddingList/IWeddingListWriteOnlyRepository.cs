namespace Ldc.Domain.Repositories.WeddingList;

public interface IWeddingListWriteOnlyRepository
{
    Task Add(Entities.WeddingList weddingList);
    Task Delete(long id);
}
