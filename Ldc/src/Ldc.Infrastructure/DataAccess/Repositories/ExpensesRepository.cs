using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class ExpensesRepository : IExpensesReadOnlyRepository, IExpensesWriteOnlyRepository, IExpensesUpdateOnlyRepository
{
    private readonly ApiDbContext _dbContext;
    
    public ExpensesRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Expense expense)
    {
        await _dbContext.Expenses.AddAsync(expense);
    }

    public async Task Delete(long id)
    {
        var result = await _dbContext.Expenses.FindAsync(id);
        
        _dbContext.Expenses.Remove(result!);
    }

    public async Task<List<Expense>> GetAll(User user)
    {
        return await _dbContext.Expenses.AsNoTracking().Where(expense => expense.UserId == user.Id).ToListAsync();
    }

    async Task<Expense?> IExpensesReadOnlyRepository.GetById(User user, long id)
    {
        return await GetFullExpense().AsNoTracking().FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == user.Id);
    }

    public async Task<List<Expense>> FilterByMonth(User user, DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;
        
        var daysInMonth =  DateTime.DaysInMonth(year: date.Year, month: date.Month);
        var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth, hour: 23, minute: 59, second: 59);
        
        return await _dbContext.Expenses.AsNoTracking().Where(expense => expense.UserId == user.Id && expense.Date >= startDate && expense.Date <= endDate).OrderBy(expense => expense.Date).ToListAsync();
    }

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetById(User user, long id)
    {
        return await  GetFullExpense().FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == user.Id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    private IIncludableQueryable<Expense, ICollection<Tag>> GetFullExpense()
    {
        return _dbContext.Expenses.Include(expense => expense.Tags);
    }
}   