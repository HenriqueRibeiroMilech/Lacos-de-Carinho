using CommonTestUtilities.Entities;
using Ldc.Domain.Entities;
using Ldc.Domain.Enums;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Test.Resource;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ExpenseIdentityManager Expense_MemberTeam { get; private set; } = default!;
    public ExpenseIdentityManager Expense_Admin { get; private set; } = default!;
    public UserIdentityManager User_Team_Member { get; private set; } = default!;
    public UserIdentityManager User_Admin { get; private set; } = default!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                
                services.AddDbContext<ApiDbContext>(config =>
                {
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });
                
                var scope =services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                var passwordEncrypter = scope.ServiceProvider.GetRequiredService<IPasswordEncrypter>();
                var accessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();
                
                StartDatabase(dbContext, passwordEncrypter, accessTokenGenerator);
            });
    }
    
    private void StartDatabase(ApiDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var userTeamMember = AddUserTeamMember(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseTeamMember =  AddExpenses(dbContext, userTeamMember, expenseId: 1, tagId: 1);
        Expense_MemberTeam = new ExpenseIdentityManager(expenseTeamMember);
        
        var userAdmin = AddUserAdmin(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseAdmin = AddExpenses(dbContext, userAdmin, expenseId: 2, tagId: 2);
        Expense_Admin = new ExpenseIdentityManager(expenseAdmin);
        
        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(ApiDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build();
        user.Id = 1;
        var password = user.Password;
        
        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);
        
        var token = accessTokenGenerator.Generate(user);
        
        User_Team_Member = new UserIdentityManager(user, password, token);
        
        return user;
    }
    
    private User AddUserAdmin(ApiDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build(Roles.ADMIN);
        user.Id = 2;
        var password = user.Password;
        
        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);
        
        var token = accessTokenGenerator.Generate(user);
        
        User_Admin = new UserIdentityManager(user, password, token);
        
        return user;
    }

    private Expense AddExpenses(ApiDbContext dbContext, User user, long expenseId, long tagId)
    {
        var expense = ExpenseBuilder.Build(user);
        expense.Id = expenseId;

        foreach (var tag in expense.Tags)
        {
            tag.Id = tagId;
            tag.ExpenseId = expenseId;
        }
        
        dbContext.Expenses.Add(expense);
        
        return expense;
    }
}