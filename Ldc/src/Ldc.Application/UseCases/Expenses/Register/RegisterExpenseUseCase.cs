using AutoMapper;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Expenses;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Expenses.Register;

public class RegisterExpenseUseCase : IRegisterExpenseUseCase
{
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    public RegisterExpenseUseCase(IExpensesWriteOnlyRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ILoggedUser loggedUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }
    public async Task<ResponseRegisterExpenseJson> Execute(RequestExpenseJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.Get();

        var expense = _mapper.Map<Expense>(request);
        expense.UserId = loggedUser.Id;
        
        await _repository.Add(expense);
        await _unitOfWork.Commit();
        
        return _mapper.Map<ResponseRegisterExpenseJson>(expense);
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