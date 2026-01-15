namespace Ldc.Domain.Repositories.GiftItem;

public interface IGiftItemWriteOnlyRepository
{
    Task Add(Entities.GiftItem giftItem);
    Task Delete(Entities.GiftItem giftItem);
}
