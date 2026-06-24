using CashFlow.Ledger.Domain.Enums;
using CashFlow.Ledger.Domain.Services;
using CashFlow.Ledger.Infrastructure.Persistence;
using CashFlow.Ledger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashFlow.Ledger.IntegrationTests.Persistence;

public sealed class EfLedgerEntryRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistLedgerEntry()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LedgerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new LedgerDbContext(options);
        var repository = new EfLedgerEntryRepository(dbContext);
        var ledgerEntry = new LedgerEntryService().Create(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale");

        // Act
        await repository.AddAsync(ledgerEntry, CancellationToken.None);
        await new EfLedgerUnitOfWork(dbContext).SaveChangesAsync(CancellationToken.None);

        // Assert
        var persistedLedgerEntry = await dbContext.LedgerEntries.SingleAsync();
        Assert.Equal(ledgerEntry.Id, persistedLedgerEntry.Id);
        Assert.Equal(150m, persistedLedgerEntry.Amount.Amount);
        Assert.Equal(EntryType.Credit, persistedLedgerEntry.EntryType);
    }
}
