namespace Ldc.Application.UseCases.GiftItems.Delete;

public interface IDeleteGiftItemUseCase
{
    Task Execute(long listId, long itemId);
}
