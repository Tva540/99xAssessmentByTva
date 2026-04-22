using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Infrastructure.Parsers;

public sealed class BalanceFileParserFactory(
    IEnumerable<IBalanceFileParser> parsers) : IBalanceFileParserFactory
{
    public IBalanceFileParser Resolve(string fileName)
    {
        return parsers.FirstOrDefault(p => p.CanParse(fileName))
               ?? throw new NotSupportedException(
                   $"Unsupported file type '{Path.GetExtension(fileName)}'. Allowed: .xlsx, .xls, .txt, .tsv");
    }
}
