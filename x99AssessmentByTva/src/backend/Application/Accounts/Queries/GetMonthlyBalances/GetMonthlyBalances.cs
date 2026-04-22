using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Helpers;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Domain.Constants;

namespace x99AssessmentByTva.Application.Accounts.Queries.GetMonthlyBalances;

#region Request and Response models
public sealed record GetMonthlyBalancesQuery(
    int Year,
    int Month) : IRequest<MonthlyBalancesDto>;

public sealed record AccountBalanceDto(
    long AccountId,
    AccountTypeCode AccountTypeCode,
    string Name,
    int DisplayOrder,
    decimal? Amount);

public sealed record MonthlyBalancesDto(
    int Year,
    int Month,
    string PeriodLabel,
    IReadOnlyList<AccountBalanceDto> Balances);
#endregion

public sealed class GetMonthlyBalancesQueryHandler(
    IApplicationDbContext context) : IRequestHandler<GetMonthlyBalancesQuery, MonthlyBalancesDto>
{
    public async Task<MonthlyBalancesDto> Handle(
        GetMonthlyBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await context.Accounts
                .Include(a => a.AccountType)
                .Include(a => a.Balances.Where(b => b.Year == request.Year && b.Month == request.Month))
            .OrderBy(a => a.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new MonthlyBalancesDto
        (
            Year: request.Year,
            Month: request.Month,
            PeriodLabel: PeriodFormatter.FormatMonthYear(request.Month, request.Year),
            Balances: accounts.Select(a => new AccountBalanceDto
            (
                AccountId: a.Id,
                AccountTypeCode: a.AccountType.Code,
                Name: a.Name,
                DisplayOrder: a.DisplayOrder,
                Amount: a.Balances.Select(b => (decimal?)b.Amount).FirstOrDefault())
            )
            .ToList()
        );
    }
}
