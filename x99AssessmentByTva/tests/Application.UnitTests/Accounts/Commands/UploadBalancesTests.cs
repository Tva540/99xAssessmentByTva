using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Text;
using x99AssessmentByTva.Application.Accounts.Commands.UploadBalances;
using x99AssessmentByTva.Infrastructure.Data;
using x99AssessmentByTva.Infrastructure.Parsers;

namespace x99AssessmentByTva.Application.UnitTests.Accounts.Commands;

public sealed class UploadBalancesTests
{
    [Theory]
    [InlineData("sample.tsv")]
    [InlineData("sample.txt")]
    [InlineData("SAMPLE.TSV")]
    [InlineData("SAMPLE.TXT")]
    public async Task Handle_ImportsTsvOrTxt_CreatesBalanceRows(string fileName)
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var (stream, _) = BuildTsvStream(
            "Account Balances for January",
            "R&D\t5.63",
            "Canteen\t50000",
            "CEO's car\t10000",
            "Marketing\t-600",
            "Parking fines\t2000");

        var result = await handler.Handle(
            new UploadBalancesCommand(stream, fileName, 2017),
            default);

        Assert.Equal(5, result.ImportedCount);
        Assert.Equal(1, result.Month);
        Assert.Equal(2017, result.Year);
        Assert.Empty(result.Warnings);
        Assert.Equal(5, await db.AccountBalances.CountAsync());
    }

    [Theory]
    [InlineData("sample.xlsx")]
    [InlineData("sample.xls")]
    [InlineData("SAMPLE.XLSX")]
    public async Task Handle_ImportsExcel_CreatesBalanceRows(string fileName)
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var stream = BuildExcelStream(
            header: "Account Balances for March",
            rows:
            [
                ("R&D", 10.56m),
                ("Canteen", 98000m),
                ("CEO's car", 24000m),
                ("Marketing", -19112m),
                ("Parking fines", 11000m)
            ]);

        var result = await handler.Handle(
            new UploadBalancesCommand(stream, fileName, 2017),
            default);

        Assert.Equal(5, result.ImportedCount);
        Assert.Equal(3, result.Month);
        Assert.Equal(5, await db.AccountBalances.CountAsync());

        var rndAccountId = await db.Accounts
            .Where(a => a.Name == "R&D")
            .Select(a => a.Id)
            .SingleAsync();
        Assert.Equal(
            10.56m,
            await db.AccountBalances
                .Where(b => b.AccountId == rndAccountId)
                .Select(b => b.Amount)
                .SingleAsync());
    }

    [Fact]
    public async Task Handle_SecondUpload_UpdatesAmounts()
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var (s1, n1) = BuildTsvStream(
            "Account Balances for January",
            "R&D\t5.00");
        var (s2, n2) = BuildTsvStream(
            "Account Balances for January",
            "R&D\t99.99");

        await handler.Handle(new UploadBalancesCommand(s1, n1, 2017), default);
        await handler.Handle(new UploadBalancesCommand(s2, n2, 2017), default);

        var stored = await db.AccountBalances.SingleAsync();
        Assert.Equal(99.99m, stored.Amount);
    }

    [Fact]
    public async Task Handle_UnknownAccountName_RecordsWarningAndSkips()
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var (stream, fileName) = BuildTsvStream(
            "Account Balances for February",
            "R&D\t1.00",
            "Bogus Account\t999");

        var result = await handler.Handle(
            new UploadBalancesCommand(stream, fileName, 2017),
            default);

        Assert.Equal(1, result.ImportedCount);
        Assert.Single(result.Warnings);
        Assert.Contains("Bogus Account", result.Warnings[0]);
        Assert.Equal(1, await db.AccountBalances.CountAsync());
    }

    [Fact]
    public async Task Handle_MissingHeader_Throws()
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var (stream, fileName) = BuildTsvStream(
            "R&D\t5.63",
            "Canteen\t50000");

        await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            handler.Handle(new UploadBalancesCommand(stream, fileName, 2017), default));
    }

    [Fact]
    public async Task Handle_EmptyFile_Throws()
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var (stream, fileName) = BuildTsvStream("");

        await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            handler.Handle(new UploadBalancesCommand(stream, fileName, 2017), default));
    }

    [Fact]
    public async Task Handle_UnsupportedExtension_Throws()
    {
        await using var db = CreateInMemoryDb();
        await SeedAccountsAsync(db);
        var handler = BuildHandler(db);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes("anything"));

        await Assert.ThrowsAsync<NotSupportedException>(() =>
            handler.Handle(new UploadBalancesCommand(stream, "sample.pdf", 2017), default));
    }

    #region Private helpers
    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task SeedAccountsAsync(ApplicationDbContext db)
    {
        await ApplicationDbContextInitialiser.SeedAccountTypesAsync(db);
        await ApplicationDbContextInitialiser.SeedAccountsAsync(db);
    }

    private static UploadBalancesCommandHandler BuildHandler(ApplicationDbContext db)
    {
        var factory = new BalanceFileParserFactory(
        [
            new TsvBalanceParser(),
            new ExcelBalanceParser()
        ]);

        return new UploadBalancesCommandHandler(db, factory);
    }

    private static (Stream stream, string fileName) BuildTsvStream(params string[] lines)
    {
        return (
            new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", lines))),
            "sample.txt");
    }

    private static MemoryStream BuildExcelStream(
        string header,
        (string Name, decimal Amount)[] rows)
    {
        var all = new List<ExcelRow>
        {
            new(header, null)
        };
        foreach (var row in rows)
            all.Add(new ExcelRow(row.Name, row.Amount));

        var ms = new MemoryStream();
        ms.SaveAs(all, printHeader: false);
        ms.Position = 0;
        return ms;
    }

    private sealed record ExcelRow(string Col1, decimal? Col2);
    #endregion
}
