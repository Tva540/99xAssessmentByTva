namespace x99AssessmentByTva.Domain.Entities;

public sealed class AccountBalance : BaseAuditableEntity
{
    public long AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
}
