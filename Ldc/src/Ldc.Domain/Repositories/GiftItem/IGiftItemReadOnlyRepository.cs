namespace Ldc.Domain.Repositories.GiftItem;

public interface IGiftItemReadOnlyRepository
{
    Task<Entities.GiftItem?> GetById(long id);
    Task<List<Entities.GiftItem>> GetByWeddingListId(long weddingListId);
}
