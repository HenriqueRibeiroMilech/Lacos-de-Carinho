using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Expenses.GetById;

public interface IGetExpenseByIdUseCase
{
    Task<ResponseExpenseJson> Execute(long id);
}