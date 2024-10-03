using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infrastructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Test.Resources;

namespace WebApi.Test;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ExpenseIdentityManager ExpenseAdmin { get; private set; } = default!;
    public ExpenseIdentityManager ExpenseMemberTeam { get; private set; } = default!;
    public UserIdentityManager UserTeamMember { get; private set; } = default!;
    public UserIdentityManager UserAdmin { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<CashFlowDbContext>(config =>
                {
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });

                var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
                var passwordEncrypter = scope.ServiceProvider.GetRequiredService<IPasswordEncrypter>();
                var accessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

                StartDatabase(dbContext, passwordEncrypter, accessTokenGenerator);
            });
    }

    private void StartDatabase(
        CashFlowDbContext dbContext, 
        IPasswordEncrypter passwordEncrypter, 
        IAccessTokenGenerator accessTokenGenerator)
    {
        var userTeamMember = AddUserTeamMember(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseMemberTeam = AddExpenses(dbContext, userTeamMember, expenseId: 1);
        ExpenseMemberTeam = new ExpenseIdentityManager(expenseMemberTeam);

        var userAdmin = AddUserAdmin(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseAdmin = AddExpenses(dbContext, userAdmin, expenseId: 2);
        ExpenseAdmin = new ExpenseIdentityManager(expenseAdmin);

        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(
        CashFlowDbContext dbContext, 
        IPasswordEncrypter passwordEncrypter, 
        IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build();
        user.Id = 1;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);

        var token = accessTokenGenerator.Generate(user);

        UserTeamMember = new UserIdentityManager(user, password, token);

        return user;
    }

    private User AddUserAdmin(
        CashFlowDbContext dbContext,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build(Roles.ADMIN);
        user.Id = 2;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);

        var token = accessTokenGenerator.Generate(user);

        UserAdmin = new UserIdentityManager(user, password, token);

        return user;
    }

    private Expense AddExpenses(CashFlowDbContext dbContext, User user, long expenseId)
    {
        var expense = ExpenseBuilder.Build(user);
        expense.Id = expenseId;

        dbContext.Expenses.Add(expense);

        return expense;
    }
}
