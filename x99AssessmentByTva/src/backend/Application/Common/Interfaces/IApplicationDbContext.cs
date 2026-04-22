using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Domain.Entities;

namespace x99AssessmentByTva.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AccountType> AccountTypes { get; }
    DbSet<Account> Accounts { get; }
    DbSet<AccountBalance> AccountBalances { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
