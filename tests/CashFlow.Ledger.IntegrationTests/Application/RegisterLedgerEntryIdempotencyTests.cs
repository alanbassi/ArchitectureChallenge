using CashFlow.Ledger.Application.LedgerEntries.Commands.Register;
using CashFlow.Ledger.Domain.Enums;
using CashFlow.Ledger.Domain.Exceptions;
using CashFlow.Ledger.Domain.Services;
using CashFlow.Ledger.Infrastructure.Persistence;
using CashFlow.Ledger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashFlow.Ledger.IntegrationTests.Application;

public sealed class RegisterLedgerEntryIdempotencyTests
{
    [Fact]
    public async Task Handle_ShouldPersistOnlyOneLedgerEntryForAnIdenticalRetry()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);
        var command = CreateCommand();

        // Act
        var firstResult = await handler.Handle(command, CancellationToken.None);
        var secondResult = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(firstResult.Id, secondResult.Id);
        Assert.Single(await dbContext.LedgerEntries.ToListAsync());
        Assert.Single(await dbContext.IdempotencyRecords.ToListAsync());
        Assert.Single(await dbContext.IntegrationMessages.ToListAsync());
    }

    [Fact]
    public async Task Handle_ShouldRejectAChangedRequestWithTheSameIdempotencyKey()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);
        var originalCommand = CreateCommand();
        var conflictingCommand = originalCommand with { Description = "Changed description" };
        await handler.Handle(originalCommand, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<IdempotencyConflictException>(async () =>
            await handler.Handle(conflictingCommand, CancellationToken.None));
        Assert.Single(await dbContext.LedgerEntries.ToListAsync());
    }

    private static LedgerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LedgerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LedgerDbContext(options);
    }

    private static RegisterLedgerEntryCommandHandler CreateHandler(LedgerDbContext dbContext) =>
        new(
            new EfLedgerEntryRepository(dbContext),
            new EfIdempotencyRecordRepository(dbContext),
            new EfIntegrationMessageRepository(dbContext),
            new EfLedgerUnitOfWork(dbContext),
            new LedgerEntryService(),
            new IdempotencyService());

    private static RegisterLedgerEntryCommand CreateCommand() =>
        new(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale",
            "key-123");
}
