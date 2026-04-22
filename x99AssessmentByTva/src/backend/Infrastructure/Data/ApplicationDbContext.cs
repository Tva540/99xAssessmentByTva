using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Domain.Entities;
using x99AssessmentByTva.Infrastructure.Identity;

namespace x99AssessmentByTva.Infrastructure.Data;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{

    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountBalance> AccountBalances => Set<AccountBalance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
