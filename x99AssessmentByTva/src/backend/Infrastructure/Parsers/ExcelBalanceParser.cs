using MiniExcelLibs;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Infrastructure.Parsers;

public sealed class ExcelBalanceParser : BalanceFileParserBase
{
    public override bool CanParse(string fileName)
    {
        return fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
               fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);
    }

    public override Task<ParsedBalanceFile> ParseAsync(
        Stream stream,
        string fileName,
        CancellationToken token = default)
    {
        var rows = stream.Query(useHeaderRow: false)
            .Cast<IDictionary<string, object>>()
            .ToList();

        int? month = null;
        var balances = new List<ParsedBalance>();

        foreach (var row in rows)
        {
            var values = row.Values
                .Where(v => v is not null)
                .Select(v => v!.ToString() ?? string.Empty)
                .ToArray();

            if (values.Length == 0) 
                continue;

            if (month is null)
            {
                var header = string.Join(" ", values);
                var parsedMonth = TryParseMonthFromHeader(header.AsSpan());
                if (parsedMonth is not null) 
                {
                    month = parsedMonth; 
                    continue; 
                }
            }

            if (values.Length < 2) 
                continue;

            var name = values[0].Trim();
            if (string.IsNullOrWhiteSpace(name)) 
                continue;

            if (!TryParseAmount(values[^1].AsSpan(), out var amount)) 
                continue;

            balances.Add(new ParsedBalance(name, amount));
        }

        return Task.FromResult(new ParsedBalanceFile(month, balances));
    }
}
