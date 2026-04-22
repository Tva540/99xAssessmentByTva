using System.Globalization;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Infrastructure.Parsers;

public abstract class BalanceFileParserBase : IBalanceFileParser
{
    public abstract bool CanParse(string fileName);

    public abstract Task<ParsedBalanceFile> ParseAsync(
        Stream stream,
        string fileName,
        CancellationToken token = default);

    #region Helpers for derived classes

    private static readonly string[] MonthNames = Enumerable.Range(1, 12)
        .Select(m => CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(m).ToLowerInvariant())
        .ToArray();

    private static readonly string[] MonthAbbreviations = Enumerable.Range(1, 12)
        .Select(m => CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(m).ToLowerInvariant())
        .ToArray();

    protected static int? TryParseMonthFromHeader(
        ReadOnlySpan<char> header)
    {
        if (header.IsWhiteSpace())
            return null;

        var remaining = header.TrimEnd();
        while (!remaining.IsEmpty)
        {
            var spaceIdx = remaining.LastIndexOfAny(' ', '\t');

            var word = spaceIdx < 0 ?
                remaining :
                remaining[(spaceIdx + 1)..];

            var month = MonthNameToNumber(word);

            if (month is not null)
                return month;

            remaining = spaceIdx < 0 ?
                [] :
                remaining[..spaceIdx].TrimEnd();
        }
        return null;
    }

    protected static int? MonthNameToNumber(
        ReadOnlySpan<char> value)
    {
        for (var i = 0; i < 12; i++)
        {
            if (value.Equals(MonthNames[i], StringComparison.OrdinalIgnoreCase) ||
                value.Equals(MonthAbbreviations[i], StringComparison.OrdinalIgnoreCase))
            {
                return i + 1;
            }
        }

        return null;
    }

    protected static bool TryParseAmount(
        ReadOnlySpan<char> raw,
        out decimal amount)
    {
        amount = 0m;

        if (raw.IsWhiteSpace())
            return false;

        Span<char> buffer = stackalloc char[raw.Length];
        var len = 0;
        foreach (var c in raw)
        {
            if (c is not (',' or '/' or '=') && !IsRsPrefix(raw, c))
                buffer[len++] = c;
        }

        return decimal.TryParse(
            buffer[..len].Trim(),
            System.Globalization.NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out amount);
    }

    //INFO: Extra validation checking if the amount contains sri lanka currency symbol or code
    private static bool IsRsPrefix(
        ReadOnlySpan<char> raw,
        char c)
    {
        //INFO: Fast-path exit - skip expensive checks if first char can't start "Rs" or "LKR"
        if (c is not ('R' or 'r' or 'S' or 's' or 'L' or 'l'))
            return false;

        var trimmed = raw.Trim();
        return trimmed.StartsWith("Rs.", StringComparison.OrdinalIgnoreCase) ||
               trimmed.StartsWith("Rs ", StringComparison.OrdinalIgnoreCase) ||
               trimmed.StartsWith("LKR.", StringComparison.OrdinalIgnoreCase) ||
               trimmed.StartsWith("LKR ", StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}
