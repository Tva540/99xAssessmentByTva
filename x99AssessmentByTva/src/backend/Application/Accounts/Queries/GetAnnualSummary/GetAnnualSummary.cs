using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Helpers;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Application.Accounts.Queries.GetAnnualSummary;

#region Request and Response models
public sealed record GetAnnualSummaryQuery(int Year) : IRequest<AnnualSummaryDto>;

public sealed record AnnualAccountColumn(
    long AccountId,
    string Name,
    int DisplayOrder,
    decimal AnnualTotal);

public sealed record AnnualMonthRow(
    int Month,
    string MonthLabel,
    IReadOnlyList<decimal?> Amounts,
    decimal MonthTotal);

public sealed record AnnualSummaryDto(
    int Year,
    IReadOnlyList<AnnualAccountColumn> Accounts,
    IReadOnlyList<AnnualMonthRow> Months,
    decimal GrandTotal);
#endregion

public sealed class GetAnnualSummaryQueryHandler(
    IApplicationDbContext context) : IRequestHandler<GetAnnualSummaryQuery, AnnualSummaryDto>
{
    public async Task<AnnualSummaryDto> Handle(
        GetAnnualSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await context.Accounts
            .OrderBy(a => a.DisplayOrder)
                .Include(a => a.Balances.Where(b => b.Year == request.Year))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var lookup = accounts
            .SelectMany(a => a.Balances.Select(b => (AccountId: a.Id, b.Month, b.Amount)))
            .ToDictionary(x => (x.AccountId, x.Month), x => x.Amount);

        return new AnnualSummaryDto
        (
            Year: request.Year,
            Accounts: accounts.Select(a => new AnnualAccountColumn
            (
                AccountId: a.Id,
                Name: a.Name,
                DisplayOrder: a.DisplayOrder,
                AnnualTotal: a.Balances.Sum(b => b.Amount))
            )
            .ToList(),
            Months: Enumerable.Range(1, 12).Select(m =>
            {
                var amounts = accounts
                    .Select(a => lookup.TryGetValue((a.Id, m), out var amt) ? (decimal?)amt : null)
                    .ToList();

                return new AnnualMonthRow(
                    Month: m,
                    MonthLabel: PeriodFormatter.FormatMonthYear(m, request.Year),
                    Amounts: amounts,
                    MonthTotal: amounts.Where(x => x.HasValue).Sum(x => x!.Value));
            })
            .ToList(),
            GrandTotal: accounts.SelectMany(a => a.Balances).Sum(b => b.Amount)
        );
    }
}
