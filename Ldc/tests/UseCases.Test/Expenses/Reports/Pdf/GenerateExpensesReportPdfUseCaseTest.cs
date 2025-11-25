using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using Ldc.Application.UseCases.Expenses.Reports.Pdf;
using Ldc.Domain.Entities;
using FluentAssertions;

namespace UseCases.Test.Expenses.Reports.Pdf;

public class GenerateExpensesReportPdfUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var expenses = ExpenseBuilder.Collection(loggedUser);
    
        var useCase = CreateUseCase(loggedUser, expenses);

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));
        
        result.Should().NotBeNullOrEmpty();

    } 
    
    [Fact]
    public async Task Success_Empty()
    {
        var loggedUser = UserBuilder.Build();
    
        var useCase = CreateUseCase(loggedUser, new List<Expense>());

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.Should().BeEmpty();
    } 
    
    private GenerateExpensesReportPdfUseCase CreateUseCase(User user, List<Expense> expenses)
    {
        var repository = new ExpensesReadOnlyRepositoryBuilder().FilterByMonth(user, expenses).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        
        return new GenerateExpensesReportPdfUseCase(repository, loggedUser);
    }
}