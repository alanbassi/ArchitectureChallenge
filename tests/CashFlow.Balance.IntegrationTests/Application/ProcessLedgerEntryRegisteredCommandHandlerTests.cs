using CashFlow.Balance.Application.DailyBalances.Commands.ProcessLedgerEntryRegistered;
using CashFlow.Balance.Domain.Services;
using CashFlow.Balance.Infrastructure.Persistence;
using CashFlow.Balance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashFlow.Balance.IntegrationTests.Application;

public sealed class ProcessLedgerEntryRegisteredCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldConsolidateCreditsAndDebitsForTheSameBusinessDate()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);
        var businessDate = new DateOnly(2026, 6, 23);

        // Act
        await handler.Handle(CreateCommand("Credit", 150m, businessDate), CancellationToken.None);
        await handler.Handle(CreateCommand("Debit", 50m, businessDate), CancellationToken.None);

        // Assert
        var balance = Assert.Single(dbContext.DailyBalances);
        Assert.Equal(150m, balance.TotalCredits);
        Assert.Equal(50m, balance.TotalDebits);
        Assert.Equal(100m, balance.Balance);
    }

    [Fact]
    public async Task Handle_ShouldNotApplyTheSameEventTwice()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);
        var command = CreateCommand("Credit", 150m, new DateOnly(2026, 6, 23));

        // Act
        await handler.Handle(command, CancellationToken.None);
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var balance = Assert.Single(dbContext.DailyBalances);
        Assert.Equal(150m, balance.TotalCredits);
        Assert.Single(dbContext.InboxMessages);
    }

    [Fact]
    public async Task Handle_ShouldUseTheOriginalBusinessDateForLateEntries()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var handler = CreateHandler(dbContext);
        var originalBusinessDate = new DateOnly(2026, 6, 20);

        // Act
        await handler.Handle(CreateCommand("Credit", 150m, originalBusinessDate), CancellationToken.None);

        // Assert
        var balance = Assert.Single(dbContext.DailyBalances);
        Assert.Equal(originalBusinessDate, balance.BusinessDate);
    }

    private static BalanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BalanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BalanceDbContext(options);
    }

    private static ProcessLedgerEntryRegisteredCommandHandler CreateHandler(BalanceDbContext dbContext) =>
        new(
            new EfDailyBalanceRepository(dbContext),
            new EfInboxMessageRepository(dbContext),
            new EfBalanceUnitOfWork(dbContext),
            new DailyBalanceService());

    private static ProcessLedgerEntryRegisteredCommand CreateCommand(
        string entryType,
        decimal amount,
        DateOnly businessDate) =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "merchant-123",
            entryType,
            amount,
            businessDate,
            new DateTimeOffset(2026, 6, 23, 14, 30, 0, TimeSpan.Zero),
            1);
}
