using Ldc.Communication.Requests;

namespace Ldc.Application.UseCases.Expenses.Update;

public interface IUpdateExpenseUseCase
{
    Task Execute(long id, RequestExpenseJson request);
}