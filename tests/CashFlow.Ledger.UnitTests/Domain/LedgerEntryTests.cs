using CashFlow.Ledger.Domain.Enums;
using CashFlow.Ledger.Domain.Services;
using Xunit;

namespace CashFlow.Ledger.UnitTests.Domain;

public sealed class LedgerEntryTests
{
    [Fact]
    public void Create_ShouldCreateValidCreditLedgerEntry()
    {
        // Arrange
        var registeredAtUtc = new DateTimeOffset(2026, 6, 23, 12, 0, 0, TimeSpan.Zero);
        var service = new LedgerEntryService();

        // Act
        var ledgerEntry = service.Create(
            "merchant-123",
            EntryType.Credit,
            150.259m,
            new DateOnly(2026, 6, 23),
            " Daily sale ",
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            registeredAtUtc);

        // Assert
        Assert.Equal(EntryType.Credit, ledgerEntry.EntryType);
        Assert.Equal(150.26m, ledgerEntry.Amount.Amount);
        Assert.Equal("Daily sale", ledgerEntry.Description);
        Assert.Equal(registeredAtUtc, ledgerEntry.RegisteredAtUtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldRejectNonPositiveAmount(decimal amount)
    {
        // Arrange
        var service = new LedgerEntryService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => service.Create(
            "merchant-123",
            EntryType.Debit,
            amount,
            new DateOnly(2026, 6, 23),
            null));
    }

    [Fact]
    public void Create_ShouldRejectDefaultBusinessDate()
    {
        // Arrange
        var service = new LedgerEntryService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.Create(
            "merchant-123",
            EntryType.Debit,
            10m,
            default,
            null));
    }
}
