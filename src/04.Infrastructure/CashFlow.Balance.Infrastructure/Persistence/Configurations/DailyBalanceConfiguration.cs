using CashFlow.Balance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Balance.Infrastructure.Persistence.Configurations;

public sealed class DailyBalanceConfiguration : IEntityTypeConfiguration<DailyBalance>
{
    public void Configure(EntityTypeBuilder<DailyBalance> builder)
    {
        builder.ToTable("DailyBalances");

        builder.HasKey(balance => balance.Id);

        builder.Property(balance => balance.MerchantId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(balance => balance.BusinessDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(balance => balance.TotalCredits).HasPrecision(18, 2).IsRequired();
        builder.Property(balance => balance.TotalDebits).HasPrecision(18, 2).IsRequired();
        builder.Property(balance => balance.Balance).HasPrecision(18, 2).IsRequired();
        builder.Property(balance => balance.UpdatedAtUtc).IsRequired();

        builder.HasIndex(balance => new { balance.MerchantId, balance.BusinessDate }).IsUnique();
    }
}
