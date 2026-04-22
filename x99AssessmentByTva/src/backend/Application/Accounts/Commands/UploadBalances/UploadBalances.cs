using Microsoft.EntityFrameworkCore;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Domain.Entities;

namespace x99AssessmentByTva.Application.Accounts.Commands.UploadBalances;

#region Request and Response models
public sealed record UploadBalancesCommand(
    Stream FileStream,
    string FileName,
    int Year) : IRequest<UploadResultDto>;

public sealed record UploadResultDto(
    int Year,
    int Month,
    int ImportedCount,
    IReadOnlyList<string> Warnings);
#endregion

public sealed class UploadBalancesCommandHandler(
    IApplicationDbContext context,
    IBalanceFileParserFactory parserFactory) : IRequestHandler<UploadBalancesCommand, UploadResultDto>
{
    public async Task<UploadResultDto> Handle(
        UploadBalancesCommand request,
        CancellationToken cancellationToken)
    {
        ParsedBalanceFile? parsed = await GetParsedBalance(
            request,
            cancellationToken);

        var month = parsed!.Month!.Value;

        var accounts = await context.Accounts
                .Include(a => a.Balances.Where(b => b.Year == request.Year && b.Month == month))
            .ToListAsync(cancellationToken);

        var warnings = new List<string>();
        var importedCount = 0;

        foreach (var row in parsed.Balances)
        {
            var account = MatchAccount(
                accounts, 
                row.AccountName);

            if (account is null)
            {
                warnings.Add($"Unknown account '{row.AccountName}' — skipped");
                continue;
            }

            var existing = account.Balances.FirstOrDefault();
            if (existing is not null)
                existing.Amount = row.Amount;
            else
            {
                context.AccountBalances.Add(new AccountBalance
                {
                    AccountId = account.Id,
                    Year = request.Year,
                    Month = month,
                    Amount = row.Amount
                });
            }

            importedCount++;
        }

        await context.SaveChangesAsync(cancellationToken);

        return new UploadResultDto(
            request.Year,
            month,
            importedCount,
            warnings);
    }

    #region Private Helpers
    private async Task<ParsedBalanceFile> GetParsedBalance(
        UploadBalancesCommand request,
        CancellationToken cancellationToken)
    {
        var parsed = await parserFactory.Resolve(request.FileName).ParseAsync(
            request.FileStream,
            request.FileName,
            cancellationToken);

        Guard.Against.Null(
            parsed.Month,
            message: "Invalid month name in file header. Expected format: 'Account Balances for January'");

        Guard.Against.Zero(
            parsed.Balances.Count,
            "balances",
            message: "No balance rows found in file");

        return parsed;
    }

    private static Account? MatchAccount(
        IReadOnlyList<Account> accounts,
        string rawName)
    {
        var normalized = Normalize(rawName);

        return accounts.FirstOrDefault(a =>
            Normalize(a.Name) == normalized ||
            Normalize(a.Name).StartsWith(normalized) ||
            normalized.StartsWith(Normalize(a.Name)));

        static string Normalize(string value)
        {
            return new string(value
                    .Where(c => !char.IsWhiteSpace(c) && c is not ('&' or '\'' or '.'))
                    .ToArray())
                    .ToLowerInvariant();
        }
    }

    #endregion
}
