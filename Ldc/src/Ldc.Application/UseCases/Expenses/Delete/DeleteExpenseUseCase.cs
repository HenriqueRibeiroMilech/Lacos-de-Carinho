using Ldc.Communication.Responses;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Expenses;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Expenses.Delete;

public class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesReadOnlyRepository _expenseReadOnly;
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteExpenseUseCase(IExpensesWriteOnlyRepository repository, IUnitOfWork unitOfWork, ILoggedUser loggedUser,
        IExpensesReadOnlyRepository expenseReadOnly)
    {
        _expenseReadOnly = expenseReadOnly;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }
    public async Task Execute(long id)
    {
        var loggedUser = await _loggedUser.Get();
        
        var expenses = await _expenseReadOnly.GetById(loggedUser, id);
        
        if (expenses is null)
        {
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }
        
        await _repository.Delete(id);
        
        await _unitOfWork.Commit();
    }
}