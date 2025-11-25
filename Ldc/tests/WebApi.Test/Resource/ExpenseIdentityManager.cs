using Ldc.Domain.Entities;

namespace WebApi.Test.Resource;

public class ExpenseIdentityManager
{
    private readonly Expense _expense;

    public ExpenseIdentityManager(Expense expense)
    {
        _expense = expense;
    }
    
    public long GetId() => _expense.Id;
    public DateTime GetDate() => _expense.Date;
}