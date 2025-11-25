using AutoMapper;
using Ldc.Communication.Requests;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Expenses;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Expenses.Update;

public class UpdateExpenseUseCase : IUpdateExpenseUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExpensesUpdateOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    
    public UpdateExpenseUseCase(IMapper mapper, IUnitOfWork unitOfWork, IExpensesUpdateOnlyRepository repository, ILoggedUser loggedUser)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _repository = repository;
        _loggedUser = loggedUser;
    }
    public async Task Execute(long id, RequestExpenseJson request)
    {
        Validate(request);
        
        var loggedUser = await _loggedUser.Get();
        
        var expense = await _repository.GetById(loggedUser, id);
        
        if (expense is null)
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        
        expense.Tags.Clear();
        
        _mapper.Map(request, expense);
        
        _repository.Update(expense);
        
        await _unitOfWork.Commit();
    }
    
    private void Validate(RequestExpenseJson request)
    {
        var validator = new ExpenseValidator();
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            var errorsMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorsMessages);
        }
    }
}