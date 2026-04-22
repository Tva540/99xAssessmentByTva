using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Infrastructure.Parsers;

public sealed class TsvBalanceParser : BalanceFileParserBase
{
    public override bool CanParse(string fileName)
    {
        return fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || 
               fileName.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase);
    }

    public override async Task<ParsedBalanceFile> ParseAsync(
        Stream stream,
        string fileName,
        CancellationToken token = default)
    {
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(token);
        var span = content.AsSpan();

        int? month = null;
        var balances = new List<ParsedBalance>();
        var isFirstLine = true;

        foreach (var lineRange in span.Split('\n'))
        {
            var line = span[lineRange].Trim();
            if (line.IsEmpty) continue;

            if (isFirstLine)
            {
                isFirstLine = false;
                month = TryParseMonthFromHeader(line);
                if (month is not null) continue;
            }

            var parsed = ParseLine(line);
            if (parsed is not null)
                balances.Add(parsed);
        }

        return new ParsedBalanceFile(month, balances);
    }

    private static ParsedBalance? ParseLine(ReadOnlySpan<char> line)
    {
        var tabIdx = line.LastIndexOf('\t');
        if (tabIdx <= 0)
        {
            var spaceIdx = line.LastIndexOf(' ');
            if (spaceIdx <= 0) return null;
            tabIdx = spaceIdx;
        }

        var name = line[..tabIdx].Trim();
        var value = line[(tabIdx + 1)..].Trim();

        return name.IsEmpty || !TryParseAmount(value, out var amount)
            ? null
            : new ParsedBalance(name.ToString(), amount);
    }
}
