using System.Text;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Infrastructure.Parsers;

namespace x99AssessmentByTva.Application.UnitTests.Files;

public sealed class TsvBalanceParserTests
{
    private readonly TsvBalanceParser _parser = new();

    [Fact]
    public void CanParse_ReturnsTrueForTxtAndTsv()
    {
        Assert.True(_parser.CanParse("sample.txt"));
        Assert.True(_parser.CanParse("sample.TXT"));
        Assert.True(_parser.CanParse("sample.tsv"));
        Assert.True(_parser.CanParse("sample.TSV"));
        Assert.False(_parser.CanParse("sample.csv"));
        Assert.False(_parser.CanParse("sample.xlsx"));
    }

    [Fact]
    public async Task ParseAsync_WithHeader_ExtractsMonthAndRows()
    {
        const string content = """
            Account Balances for January
            R&D	5.63
            Canteen	50000
            CEO's car	10000
            Marketing	-600
            Parking fines	2000
            """;

        var result = await ParseAsync(content);

        Assert.Equal(1, result.Month);
        Assert.Equal(5, result.Balances.Count);
        Assert.Equal(5.63m, result.Balances[0].Amount);
        Assert.Equal("R&D", result.Balances[0].AccountName);
        Assert.Equal(-600m, result.Balances.Single(b => b.AccountName == "Marketing").Amount);
    }

    [Fact]
    public async Task ParseAsync_WithoutHeader_ReturnsNullMonth()
    {
        const string content = "R&D\t5.63\nCanteen\t50000";
        var result = await ParseAsync(content);

        Assert.Null(result.Month);
        Assert.Equal(
            2,
            result.Balances.Count);
    }

    [Fact]
    public async Task ParseAsync_HandlesCommaFormattedNumbers()
    {
        const string content = """
            Account Balances for March
            Canteen	98,000
            """;

        var result = await ParseAsync(content);

        Assert.Equal(3, result.Month);
        Assert.Equal(98_000m, result.Balances.Single().Amount);
    }

    private async Task<ParsedBalanceFile> ParseAsync(string content)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await _parser.ParseAsync(
            stream,
            "sample.txt");
    }
}
