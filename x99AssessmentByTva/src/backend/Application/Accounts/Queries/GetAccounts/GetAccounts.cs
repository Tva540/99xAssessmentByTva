using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Domain.Constants;

namespace x99AssessmentByTva.Application.Accounts.Queries.GetAccounts;

#region Request and Response models
public sealed record GetAccountsQuery : IRequest<IReadOnlyList<AccountDto>>;

public sealed record AccountDto(
    long Id,
    AccountTypeCode AccountTypeCode,
    string Name,
    int DisplayOrder);
#endregion

public sealed class GetAccountsQueryHandler(
    IApplicationDbContext context) : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountDto>>
{
    public async Task<IReadOnlyList<AccountDto>> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Accounts
                .Include(a => a.AccountType)
                .OrderBy(a => a.DisplayOrder)
                .Select(a => new AccountDto(
                    a.Id,
                    a.AccountType.Code,
                    a.Name,
                    a.DisplayOrder))
                .ToListAsync(cancellationToken);
    }
}
