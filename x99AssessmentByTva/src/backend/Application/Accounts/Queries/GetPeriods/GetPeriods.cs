using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Helpers;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Application.Accounts.Queries.GetPeriods;

#region Request and Response models
public sealed record GetPeriodsQuery : IRequest<IReadOnlyList<BalancePeriodDto>>;

public sealed record BalancePeriodDto(
    int Year,
    int Month)
{
    public string Label { get; } = PeriodFormatter.FormatMonthYear(Month, Year);
};
#endregion

public sealed class GetPeriodsQueryHandler(
    IApplicationDbContext context) : IRequestHandler<GetPeriodsQuery, IReadOnlyList<BalancePeriodDto>>
{
    public async Task<IReadOnlyList<BalancePeriodDto>> Handle(
        GetPeriodsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.AccountBalances
            .Select(b => new { b.Year, b.Month })
            .Distinct()
            .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
            .Select(p => new BalancePeriodDto(p.Year, p.Month))
            .ToListAsync(cancellationToken);
    }
}
