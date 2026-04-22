namespace x99AssessmentByTva.Domain.Entities;

public sealed class AccountType : BaseAuditableEntity
{
    public AccountTypeCode Code { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<Account> Accounts { get; set; } = [];
}
