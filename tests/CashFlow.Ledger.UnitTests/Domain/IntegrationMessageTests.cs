using CashFlow.Ledger.Domain.Entities;
using Xunit;

namespace CashFlow.Ledger.UnitTests.Domain;

public sealed class IntegrationMessageTests
{
    [Fact]
    public void Create_ShouldCreatePendingMessage()
    {
        // Arrange
        var occurredAtUtc = new DateTimeOffset(2026, 6, 23, 14, 30, 0, TimeSpan.Zero);

        // Act
        var message = IntegrationMessage.Create(
            Guid.NewGuid(),
            "LedgerEntryRegistered",
            "{\"eventId\":\"event-123\"}",
            occurredAtUtc);

        // Assert
        Assert.Equal("LedgerEntryRegistered", message.Type);
        Assert.Equal(occurredAtUtc, message.OccurredAtUtc);
        Assert.Null(message.PublishedAtUtc);
    }
}
