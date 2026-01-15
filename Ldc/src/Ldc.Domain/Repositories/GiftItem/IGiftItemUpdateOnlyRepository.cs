namespace Ldc.Domain.Repositories.GiftItem;

public interface IGiftItemUpdateOnlyRepository
{
    Task<Entities.GiftItem?> GetById(long id);
    void Update(Entities.GiftItem giftItem);
}
