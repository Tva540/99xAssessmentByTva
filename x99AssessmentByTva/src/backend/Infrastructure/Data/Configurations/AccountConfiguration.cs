using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using x99AssessmentByTva.Domain.Entities;

namespace x99AssessmentByTva.Infrastructure.Data.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasOne(x => x.AccountType)
               .WithMany(x => x.Accounts)
               .HasForeignKey(x => x.AccountTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
