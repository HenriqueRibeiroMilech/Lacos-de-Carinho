using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Expenses.Delete;

public interface IDeleteExpenseUseCase
{
    Task Execute(long id);
}