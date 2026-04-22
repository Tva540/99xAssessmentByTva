using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using x99AssessmentByTva.Domain.Constants;
using x99AssessmentByTva.Domain.Entities;
using x99AssessmentByTva.Infrastructure.Identity;

namespace x99AssessmentByTva.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public sealed class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
                await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedAccountTypesAsync(context);
        await SeedAccountsAsync(context);
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[] { Roles.Admin, Roles.Viewer })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private async Task SeedUsersAsync()
    {
        await EnsureUserAsync(
            "admin@jondell.local",
            "Admin@12345",
            "Admin User",
            Roles.Admin);

        await EnsureUserAsync(
            "viewer@jondell.local",
            "Viewer@12345",
            "Viewer User",
            Roles.Viewer);
    }

    private async Task EnsureUserAsync(
        string email,
        string password,
        string displayName,
        string role)
    {
        if (await userManager.Users.AllAsync(u => u.Email != email))
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName
            };

            var result = await userManager.CreateAsync(
                user,
                password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(
                user,
                role);
        }
    }

    public static async Task SeedAccountTypesAsync(
        ApplicationDbContext context)
    {
        if (await context.AccountTypes.AnyAsync())
            return;

        await context.AccountTypes.AddRangeAsync
        (
            new AccountType { Code = AccountTypeCode.ResearchAndDevelopment, Description = "Research & Development" },
            new AccountType { Code = AccountTypeCode.Canteen, Description = "Canteen" },
            new AccountType { Code = AccountTypeCode.CeoCar, Description = "CEO's car expenses" },
            new AccountType { Code = AccountTypeCode.Marketing, Description = "Marketing" },
            new AccountType { Code = AccountTypeCode.ParkingFines, Description = "Parking fines" }
        );

        await context.SaveChangesAsync();
    }

    public static async Task SeedAccountsAsync(
        ApplicationDbContext context)
    {
        if (await context.Accounts.AnyAsync())
            return;

        var typesByCode = await context.AccountTypes
            .ToDictionaryAsync(t => t.Code);

        await context.Accounts.AddRangeAsync
        (
            new Account { AccountTypeId = typesByCode[AccountTypeCode.ResearchAndDevelopment].Id, Name = "R&D", DisplayOrder = 1 },
            new Account { AccountTypeId = typesByCode[AccountTypeCode.Canteen].Id, Name = "Canteen", DisplayOrder = 2 },
            new Account { AccountTypeId = typesByCode[AccountTypeCode.CeoCar].Id, Name = "CEO's car expenses", DisplayOrder = 3 },
            new Account { AccountTypeId = typesByCode[AccountTypeCode.Marketing].Id, Name = "Marketing", DisplayOrder = 4 },
            new Account { AccountTypeId = typesByCode[AccountTypeCode.ParkingFines].Id, Name = "Parking fines", DisplayOrder = 5 }
        );

        await context.SaveChangesAsync();
    }
}
