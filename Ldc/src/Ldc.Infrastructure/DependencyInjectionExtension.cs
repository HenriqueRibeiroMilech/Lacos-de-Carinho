using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Expenses;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Infrastructure.Extensions;
using Ldc.Infrastructure.DataAccess;
using Ldc.Infrastructure.DataAccess.Repositories;
using Ldc.Infrastructure.Security.Tokens;
using Ldc.Infrastructure.Services.LoggedUser;
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
        services.AddScoped<IExpensesReadOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesWriteOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesUpdateOnlyRepository, ExpensesRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
        
    }
    
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");
        
        var serverVersion = ServerVersion.AutoDetect(connectionString);
            
        services.AddDbContext<ApiDbContext>(config => config.UseMySql(connectionString, serverVersion));
    }
}