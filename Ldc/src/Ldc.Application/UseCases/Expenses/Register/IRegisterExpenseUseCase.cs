using Ldc.Communication.Requests;
using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.Expenses.Register;

public interface IRegisterExpenseUseCase
{
    Task<ResponseRegisterExpenseJson> Execute(RequestExpenseJson request);
}