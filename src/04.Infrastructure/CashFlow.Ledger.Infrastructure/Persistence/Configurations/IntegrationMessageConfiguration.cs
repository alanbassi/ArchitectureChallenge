using CashFlow.Ledger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Ledger.Infrastructure.Persistence.Configurations;

public sealed class IntegrationMessageConfiguration : IEntityTypeConfiguration<IntegrationMessage>
{
    public void Configure(EntityTypeBuilder<IntegrationMessage> builder)
    {
        builder.ToTable("IntegrationMessages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Type)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(message => message.Payload)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(message => message.OccurredAtUtc)
            .IsRequired();

        builder.Property(message => message.PublishedAtUtc);

        builder.HasIndex(message => message.PublishedAtUtc);
    }
}
