namespace Ldc.Domain.Repositories.WeddingList;

public interface IWeddingListUpdateOnlyRepository
{
    Task<Entities.WeddingList?> GetById(long id);
    void Update(Entities.WeddingList weddingList);
}
