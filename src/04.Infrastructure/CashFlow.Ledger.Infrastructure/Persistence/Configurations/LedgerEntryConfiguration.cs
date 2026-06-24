using CashFlow.Ledger.Domain.Entities;
using CashFlow.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Ledger.Infrastructure.Persistence.Configurations;

public sealed class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.MerchantId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entry => entry.EntryType)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(entry => entry.Amount)
            .HasConversion(
                money => money.Amount,
                amount => Money.Create(amount))
            .HasColumnName("Amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(entry => entry.BusinessDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(entry => entry.Description)
            .HasMaxLength(500);

        builder.Property(entry => entry.RegisteredAtUtc)
            .IsRequired();

        builder.HasIndex(entry => new { entry.MerchantId, entry.BusinessDate });
    }
}
