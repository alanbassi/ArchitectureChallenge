using CashFlow.Ledger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Ledger.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");

        builder.HasKey(record => record.Id);

        builder.Property(record => record.MerchantId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(record => record.Key)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(record => record.RequestFingerprint)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(record => record.LedgerEntryId)
            .IsRequired();

        builder.Property(record => record.RegisteredAtUtc)
            .IsRequired();

        builder.HasIndex(record => new { record.MerchantId, record.Key })
            .IsUnique();
    }
}
