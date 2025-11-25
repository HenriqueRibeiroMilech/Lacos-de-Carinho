using Ldc.Domain.Entities;

namespace Ldc.Domain.Repositories.Expenses;

public interface IExpensesWriteOnlyRepository
{
    Task Add(Expense expense);
    Task Delete(long id);
}