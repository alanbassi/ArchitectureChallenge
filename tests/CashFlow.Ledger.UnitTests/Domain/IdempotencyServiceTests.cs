using CashFlow.Ledger.Domain.Enums;
using CashFlow.Ledger.Domain.Services;
using Xunit;

namespace CashFlow.Ledger.UnitTests.Domain;

public sealed class IdempotencyServiceTests
{
    [Fact]
    public void CreateRequestFingerprint_ShouldBeEqualForTheSameRequest()
    {
        // Arrange
        var service = new IdempotencyService();

        // Act
        var firstFingerprint = service.CreateRequestFingerprint(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale");
        var secondFingerprint = service.CreateRequestFingerprint(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale");

        // Assert
        Assert.Equal(firstFingerprint, secondFingerprint);
    }

    [Fact]
    public void CreateRequestFingerprint_ShouldBeDifferentWhenTheRequestChanges()
    {
        // Arrange
        var service = new IdempotencyService();

        // Act
        var firstFingerprint = service.CreateRequestFingerprint(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale");
        var secondFingerprint = service.CreateRequestFingerprint(
            "merchant-123",
            EntryType.Debit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale");

        // Assert
        Assert.NotEqual(firstFingerprint, secondFingerprint);
    }
}
