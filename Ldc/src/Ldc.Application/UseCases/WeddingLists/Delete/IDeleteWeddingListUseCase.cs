namespace Ldc.Application.UseCases.WeddingLists.Delete;

public interface IDeleteWeddingListUseCase
{
    Task Execute(long id);
}
