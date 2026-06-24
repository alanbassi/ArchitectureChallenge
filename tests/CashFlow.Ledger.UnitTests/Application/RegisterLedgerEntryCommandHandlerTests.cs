using CashFlow.Ledger.Application.LedgerEntries.Commands.Register;
using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Enums;
using CashFlow.Ledger.Domain.Exceptions;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Domain.Services;
using System.Text.Json;
using Xunit;

namespace CashFlow.Ledger.UnitTests.Application;

public sealed class RegisterLedgerEntryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistLedgerEntryAndReturnResult()
    {
        // Arrange
        var ledgerEntryRepository = new InMemoryLedgerEntryRepository();
        var integrationMessageRepository = new InMemoryIntegrationMessageRepository();
        var handler = CreateHandler(
            ledgerEntryRepository,
            new InMemoryIdempotencyRecordRepository(),
            integrationMessageRepository);
        var command = new RegisterLedgerEntryCommand(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale",
            "key-123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var persistedLedgerEntry = Assert.Single(ledgerEntryRepository.LedgerEntries);
        Assert.Equal(persistedLedgerEntry.Id, result.Id);
        Assert.Equal(150m, result.Amount);
        Assert.Equal("Daily sale", result.Description);
        var integrationMessage = Assert.Single(integrationMessageRepository.Messages);
        Assert.Equal("LedgerEntryRegistered", integrationMessage.Type);
        Assert.Null(integrationMessage.PublishedAtUtc);
        using var payload = JsonDocument.Parse(integrationMessage.Payload);
        Assert.Equal(result.Id, payload.RootElement.GetProperty("ledgerEntryId").GetGuid());
        Assert.Equal(1, payload.RootElement.GetProperty("contractVersion").GetInt32());
    }

    [Fact]
    public async Task Handle_ShouldReturnOriginalResultForTheSameIdempotencyKeyAndRequest()
    {
        // Arrange
        var ledgerEntryRepository = new InMemoryLedgerEntryRepository();
        var integrationMessageRepository = new InMemoryIntegrationMessageRepository();
        var handler = CreateHandler(
            ledgerEntryRepository,
            new InMemoryIdempotencyRecordRepository(),
            integrationMessageRepository);
        var command = new RegisterLedgerEntryCommand(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale",
            "key-123");

        // Act
        var firstResult = await handler.Handle(command, CancellationToken.None);
        var secondResult = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(ledgerEntryRepository.LedgerEntries);
        Assert.Single(integrationMessageRepository.Messages);
        Assert.Equal(firstResult.Id, secondResult.Id);
    }

    [Fact]
    public async Task Handle_ShouldRejectSameIdempotencyKeyWithDifferentRequest()
    {
        // Arrange
        var ledgerEntryRepository = new InMemoryLedgerEntryRepository();
        var handler = CreateHandler(
            ledgerEntryRepository,
            new InMemoryIdempotencyRecordRepository(),
            new InMemoryIntegrationMessageRepository());
        var originalCommand = new RegisterLedgerEntryCommand(
            "merchant-123",
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            "Daily sale",
            "key-123");
        var conflictingCommand = originalCommand with { Amount = 151m };
        await handler.Handle(originalCommand, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<IdempotencyConflictException>(async () =>
            await handler.Handle(conflictingCommand, CancellationToken.None));
    }

    [Fact]
    public void Validate_ShouldRejectCommandWithoutMerchant()
    {
        // Arrange
        var validator = new RegisterLedgerEntryCommandValidator();
        var command = new RegisterLedgerEntryCommand(
            string.Empty,
            EntryType.Credit,
            150m,
            new DateOnly(2026, 6, 23),
            null,
            "key-123");

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
    }

    private static RegisterLedgerEntryCommandHandler CreateHandler(
        InMemoryLedgerEntryRepository ledgerEntryRepository,
        InMemoryIdempotencyRecordRepository idempotencyRecordRepository,
        InMemoryIntegrationMessageRepository integrationMessageRepository) =>
        new(
            ledgerEntryRepository,
            idempotencyRecordRepository,
            integrationMessageRepository,
            new InMemoryLedgerUnitOfWork(),
            new LedgerEntryService(),
            new IdempotencyService());

    private sealed class InMemoryLedgerEntryRepository : ILedgerEntryRepository
    {
        public List<LedgerEntry> LedgerEntries { get; } = [];

        public Task<LedgerEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(LedgerEntries.SingleOrDefault(entry => entry.Id == id));

        public Task<IReadOnlyList<LedgerEntry>> GetByMerchantAsync(
            string merchantId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<LedgerEntry>>(
                LedgerEntries.Where(entry => entry.MerchantId == merchantId).ToList());

        public Task AddAsync(LedgerEntry ledgerEntry, CancellationToken cancellationToken)
        {
            LedgerEntries.Add(ledgerEntry);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryIdempotencyRecordRepository : IIdempotencyRecordRepository
    {
        public List<IdempotencyRecord> Records { get; } = [];

        public Task<IdempotencyRecord?> GetByMerchantAndKeyAsync(
            string merchantId,
            string key,
            CancellationToken cancellationToken) =>
            Task.FromResult(Records.SingleOrDefault(record =>
                record.MerchantId == merchantId.Trim() && record.Key == key.Trim()));

        public Task AddAsync(IdempotencyRecord idempotencyRecord, CancellationToken cancellationToken)
        {
            Records.Add(idempotencyRecord);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryLedgerUnitOfWork : ILedgerUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class InMemoryIntegrationMessageRepository : IIntegrationMessageRepository
    {
        public List<IntegrationMessage> Messages { get; } = [];

        public Task<IReadOnlyList<IntegrationMessage>> GetPendingAsync(
            int maximumCount,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<IntegrationMessage>>(
                Messages.Where(message => message.PublishedAtUtc is null)
                    .Take(maximumCount)
                    .ToList());

        public Task AddAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken)
        {
            Messages.Add(integrationMessage);
            return Task.CompletedTask;
        }
    }
}
