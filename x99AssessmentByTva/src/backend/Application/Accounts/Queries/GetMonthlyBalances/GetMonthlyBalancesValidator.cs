namespace x99AssessmentByTva.Application.Accounts.Queries.GetMonthlyBalances;

public sealed class GetMonthlyBalancesValidator : AbstractValidator<GetMonthlyBalancesQuery>
{
    public GetMonthlyBalancesValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(
                1900,
                2200).WithMessage("Year must be between 1900 and 2200");

        RuleFor(x => x.Month)
            .InclusiveBetween(
                1,
                12).WithMessage("Month must be between 1 and 12");
    }
}
