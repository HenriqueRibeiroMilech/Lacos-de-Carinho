using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Expenses.GetAll;

public interface IGetAllExpensesUseCase
{
    Task<ResponseExpensesJson> Execute();
}