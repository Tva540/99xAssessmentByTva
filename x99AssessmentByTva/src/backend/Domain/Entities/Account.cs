namespace x99AssessmentByTva.Domain.Entities;

public sealed class Account : BaseAuditableEntity
{
    public long AccountTypeId { get; set; }
    public AccountType AccountType { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<AccountBalance> Balances { get; set; } = [];
}
