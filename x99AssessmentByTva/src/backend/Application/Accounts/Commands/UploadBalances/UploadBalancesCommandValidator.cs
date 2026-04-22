namespace x99AssessmentByTva.Application.Accounts.Commands.UploadBalances;

public sealed class UploadBalancesCommandValidator : AbstractValidator<UploadBalancesCommand>
{
    private static readonly string[] AllowedExtensions = [".xlsx", ".xls", ".txt", ".tsv"];

    public UploadBalancesCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File is required")
            .Must(f => AllowedExtensions.Any(ext =>
                f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Only .xlsx, .xls, .txt, and .tsv files are allowed");

        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("Year is required")
            .InclusiveBetween(1900, 2200)
            .WithMessage("Year must be between 1900 and 2200");
    }
}
