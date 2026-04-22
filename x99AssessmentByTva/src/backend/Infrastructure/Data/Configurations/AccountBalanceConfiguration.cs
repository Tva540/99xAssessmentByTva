using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using x99AssessmentByTva.Domain.Entities;

namespace x99AssessmentByTva.Infrastructure.Data.Configurations;

public sealed class AccountBalanceConfiguration : IEntityTypeConfiguration<AccountBalance>
{
    public void Configure(EntityTypeBuilder<AccountBalance> builder)
    {
        builder.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        builder.HasIndex(x => new { x.AccountId, x.Year, x.Month }).IsUnique();
        builder.HasOne(x => x.Account)
               .WithMany(x => x.Balances)
               .HasForeignKey(x => x.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
