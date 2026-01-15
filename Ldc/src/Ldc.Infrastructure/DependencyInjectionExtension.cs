using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Expenses;
using Ldc.Domain.Repositories.GiftItem;
using Ldc.Domain.Repositories.PasswordReset;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Repositories.TemplateItem;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Services.Email;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Domain.Services.Payment;
using Ldc.Infrastructure.Extensions;
using Ldc.Infrastructure.DataAccess;
using Ldc.Infrastructure.DataAccess.Repositories;
using Ldc.Infrastructure.Security.Tokens;
using Ldc.Infrastructure.Services.Email;
using Ldc.Infrastructure.Services.LoggedUser;
using Ldc.Infrastructure.Services.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ldc.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordEncrypter, Infrastructure.Security.Cryptography.BCrypt>();
        services.AddScoped<ILoggedUser, LoggedUser>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddHttpClient<IMercadoPagoService, MercadoPagoService>();
        
        AddToken(services, configuration);
        AddRepositories(services);

        if (configuration.IsTestEnvironment() == false)
        {
            AddDbContext(services, configuration);
        }
    }

    private static void AddToken(IServiceCollection services, IConfiguration configuration)
    {
        var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpiresMinutes");
        var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

        services.AddScoped<IAccessTokenGenerator>(config => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
    }
    
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Expense repositories
        services.AddScoped<IExpensesReadOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesWriteOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesUpdateOnlyRepository, ExpensesRepository>();
        
        // User repositories
        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
        
        // WeddingList repositories
        services.AddScoped<IWeddingListReadOnlyRepository, WeddingListRepository>();
        services.AddScoped<IWeddingListWriteOnlyRepository, WeddingListRepository>();
        services.AddScoped<IWeddingListUpdateOnlyRepository, WeddingListRepository>();
        
        // GiftItem repositories
        services.AddScoped<IGiftItemReadOnlyRepository, GiftItemRepository>();
        services.AddScoped<IGiftItemWriteOnlyRepository, GiftItemRepository>();
        services.AddScoped<IGiftItemUpdateOnlyRepository, GiftItemRepository>();
        
        // Rsvp repositories
        services.AddScoped<IRsvpReadOnlyRepository, RsvpRepository>();
        services.AddScoped<IRsvpWriteOnlyRepository, RsvpRepository>();
        services.AddScoped<IRsvpUpdateOnlyRepository, RsvpRepository>();
        
        // TemplateItem repositories
        services.AddScoped<ITemplateItemReadOnlyRepository, TemplateItemRepository>();
        
        // PasswordResetToken repository
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        
        // Payment repository
        services.AddScoped<IPaymentRepository, PaymentRepository>();
    }
    
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");
        
        var serverVersion = ServerVersion.AutoDetect(connectionString);
            
        services.AddDbContext<ApiDbContext>(config => config.UseMySql(connectionString, serverVersion));
    }
}