namespace x99AssessmentByTva.Application.Common.Interfaces;

/// <summary>
/// Parses balance files into a common <see cref="ParsedBalanceFile"/> representation.
/// Implementations handle specific file formats (e.g. Excel, TSV) and are resolved
/// via <see cref="CanParse(string)"/>.
/// </summary>
public interface IBalanceFileParser
{
    /// <summary>
    /// Returns <c>true</c> if this parser can handle the given file, 
    /// based on its extension.
    /// </summary>
    bool CanParse(string fileName);

    /// <summary>
    /// Reads the file stream and returns the parsed balance data.
    /// </summary>
    Task<ParsedBalanceFile> ParseAsync(
        Stream stream,
        string fileName,
        CancellationToken token = default);
}

public interface IBalanceFileParserFactory
{
    IBalanceFileParser Resolve(string fileName);
}

public sealed record ParsedBalance(
    string AccountName,
    decimal Amount);

public sealed record ParsedBalanceFile(
    int? Month,
    IReadOnlyList<ParsedBalance> Balances);
