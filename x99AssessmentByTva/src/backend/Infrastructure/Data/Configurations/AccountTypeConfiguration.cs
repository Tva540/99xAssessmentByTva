using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using x99AssessmentByTva.Domain.Entities;

namespace x99AssessmentByTva.Infrastructure.Data.Configurations;

public sealed class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
