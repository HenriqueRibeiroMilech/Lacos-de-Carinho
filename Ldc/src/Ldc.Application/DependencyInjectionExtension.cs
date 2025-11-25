using Ldc.Application.UseCases.Users.Login;
using Ldc.Application.AutoMapper;
using Ldc.Application.UseCases.Expenses.Delete;
using Ldc.Application.UseCases.Expenses.GetAll;
using Ldc.Application.UseCases.Expenses.GetById;
using Ldc.Application.UseCases.Expenses.Register;
using Ldc.Application.UseCases.Expenses.Reports.Excel;
using Ldc.Application.UseCases.Expenses.Reports.Pdf;
using Ldc.Application.UseCases.Expenses.Update;
using Ldc.Application.UseCases.Login.DoLogin;
using Ldc.Application.UseCases.Users.ChangePassword;
using Ldc.Application.UseCases.Users.Delete;
using Ldc.Application.UseCases.Users.Login.DoLogin;
using Ldc.Application.UseCases.Users.Profile;
using Ldc.Application.UseCases.Users.Register;
using Ldc.Application.UseCases.Users.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Ldc.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        AddAutoMapper(services);
        AddUseCases(services);
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapping));
    }
    
    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterExpenseUseCase, RegisterExpenseUseCase>();
        services.AddScoped<IGetAllExpensesUseCase, GetAllExpensesUseCase>();
        services.AddScoped<IGetExpenseByIdUseCase, GetExpenseByIdUseCase>();
        services.AddScoped<IDeleteExpenseUseCase, DeleteExpenseUseCase>();
        services.AddScoped<IUpdateExpenseUseCase, UpdateExpenseUseCase>();
        services.AddScoped<IGenerateExpensesReportExcelUseCase, GenerateExpensesReportExcelUseCase>();
        services.AddScoped<IGenerateExpensesReportPdfUseCase, GenerateExpensesReportPdfUseCase>();
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        services.AddScoped<IDeleteUserAccountUseCase, DeleteUserAccountUseCase>();
        
    }
}