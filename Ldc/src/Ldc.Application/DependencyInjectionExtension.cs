using Ldc.Application.UseCases.Users.Login;
using Ldc.Application.AutoMapper;
using Ldc.Application.UseCases.Expenses.Delete;
using Ldc.Application.UseCases.Expenses.GetAll;
using Ldc.Application.UseCases.Expenses.GetById;
using Ldc.Application.UseCases.Expenses.Register;
using Ldc.Application.UseCases.Expenses.Reports.Excel;
using Ldc.Application.UseCases.Expenses.Reports.Pdf;
using Ldc.Application.UseCases.Expenses.Update;
using Ldc.Application.UseCases.GiftItems.Add;
using Ldc.Application.UseCases.GiftItems.CancelReservation;
using Ldc.Application.UseCases.GiftItems.Delete;
using Ldc.Application.UseCases.GiftItems.Reserve;
using Ldc.Application.UseCases.GiftItems.Update;
using Ldc.Application.UseCases.Guest.GetDetails;
using Ldc.Application.UseCases.Login.DoLogin;
using Ldc.Application.UseCases.Password.RequestReset;
using Ldc.Application.UseCases.Password.ResetPassword;
using Ldc.Application.UseCases.Rsvps.GetByWeddingList;
using Ldc.Application.UseCases.Rsvps.Reports.Pdf;
using Ldc.Application.UseCases.Rsvps.Upsert;
using Ldc.Application.UseCases.TemplateItems.GetAll;
using Ldc.Application.UseCases.Users.ChangePassword;
using Ldc.Application.UseCases.Users.Delete;
using Ldc.Application.UseCases.Users.Login.DoLogin;
using Ldc.Application.UseCases.Users.Profile;
using Ldc.Application.UseCases.Users.Register;
using Ldc.Application.UseCases.Users.Update;
using Ldc.Application.UseCases.Users.Upgrade;
using Ldc.Application.UseCases.WeddingLists.Create;
using Ldc.Application.UseCases.WeddingLists.Delete;
using Ldc.Application.UseCases.WeddingLists.GetAll;
using Ldc.Application.UseCases.WeddingLists.GetById;
using Ldc.Application.UseCases.WeddingLists.GetByLink;
using Ldc.Application.UseCases.WeddingLists.GetTracking;
using Ldc.Application.UseCases.WeddingLists.RegenerateLink;
using Ldc.Application.UseCases.WeddingLists.Update;
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
        // Expense UseCases
        services.AddScoped<IRegisterExpenseUseCase, RegisterExpenseUseCase>();
        services.AddScoped<IGetAllExpensesUseCase, GetAllExpensesUseCase>();
        services.AddScoped<IGetExpenseByIdUseCase, GetExpenseByIdUseCase>();
        services.AddScoped<IDeleteExpenseUseCase, DeleteExpenseUseCase>();
        services.AddScoped<IUpdateExpenseUseCase, UpdateExpenseUseCase>();
        services.AddScoped<IGenerateExpensesReportExcelUseCase, GenerateExpensesReportExcelUseCase>();
        services.AddScoped<IGenerateExpensesReportPdfUseCase, GenerateExpensesReportPdfUseCase>();
        
        // User UseCases
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        services.AddScoped<IDeleteUserAccountUseCase, DeleteUserAccountUseCase>();
        services.AddScoped<IUpgradeUserUseCase, UpgradeUserUseCase>();
        
        // WeddingList UseCases
        services.AddScoped<ICreateWeddingListUseCase, CreateWeddingListUseCase>();
        services.AddScoped<IGetAllWeddingListsUseCase, GetAllWeddingListsUseCase>();
        services.AddScoped<IGetWeddingListByIdUseCase, GetWeddingListByIdUseCase>();
        services.AddScoped<IGetWeddingListByLinkUseCase, GetWeddingListByLinkUseCase>();
        services.AddScoped<IUpdateWeddingListUseCase, UpdateWeddingListUseCase>();
        services.AddScoped<IDeleteWeddingListUseCase, DeleteWeddingListUseCase>();
        services.AddScoped<IRegenerateLinkUseCase, RegenerateLinkUseCase>();
        services.AddScoped<IGetTrackingUseCase, GetTrackingUseCase>();
        
        // GiftItem UseCases
        services.AddScoped<IAddGiftItemUseCase, AddGiftItemUseCase>();
        services.AddScoped<IDeleteGiftItemUseCase, DeleteGiftItemUseCase>();
        services.AddScoped<IReserveGiftItemUseCase, ReserveGiftItemUseCase>();
        services.AddScoped<ICancelReservationUseCase, CancelReservationUseCase>();
        services.AddScoped<IUpdateGiftItemUseCase, UpdateGiftItemUseCase>();
        
        // Rsvp UseCases
        services.AddScoped<IUpsertRsvpUseCase, UpsertRsvpUseCase>();
        services.AddScoped<IGetRsvpsByWeddingListUseCase, GetRsvpsByWeddingListUseCase>();
        services.AddScoped<IGenerateGuestListPdfUseCase, GenerateGuestListPdfUseCase>();
        
        // Guest UseCases
        services.AddScoped<IGetGuestDetailsUseCase, GetGuestDetailsUseCase>();
        
        // TemplateItems UseCases
        services.AddScoped<IGetAllTemplateItemsUseCase, GetAllTemplateItemsUseCase>();
        
        // Password UseCases
        services.AddScoped<IRequestPasswordResetUseCase, RequestPasswordResetUseCase>();
        services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
    }
}