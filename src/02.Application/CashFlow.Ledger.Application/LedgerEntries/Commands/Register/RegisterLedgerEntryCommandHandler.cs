using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Exceptions;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Domain.Services;
using Mediator;
using CashFlow.SharedKernel.IntegrationEvents;
using System.Text.Json;

namespace CashFlow.Ledger.Application.LedgerEntries.Commands.Register;

public sealed class RegisterLedgerEntryCommandHandler(
    ILedgerEntryRepository ledgerEntryRepository,
    IIdempotencyRecordRepository idempotencyRecordRepository,
    IIntegrationMessageRepository integrationMessageRepository,
    ILedgerUnitOfWork unitOfWork,
    LedgerEntryService ledgerEntryService,
    IdempotencyService idempotencyService)
    : ICommandHandler<RegisterLedgerEntryCommand, RegisterLedgerEntryResult>
{
    public async ValueTask<RegisterLedgerEntryResult> Handle(
        RegisterLedgerEntryCommand command,
        CancellationToken cancellationToken)
    {
        var requestFingerprint = idempotencyService.CreateRequestFingerprint(
            command.MerchantId,
            command.EntryType,
            command.Amount,
            command.BusinessDate,
            command.Description);
        var existingRecord = await idempotencyRecordRepository.GetByMerchantAndKeyAsync(
            command.MerchantId,
            command.IdempotencyKey,
            cancellationToken);

        if (existingRecord is not null)
        {
            if (!string.Equals(existingRecord.RequestFingerprint, requestFingerprint, StringComparison.Ordinal))
            {
                throw new IdempotencyConflictException();
            }

            var originalLedgerEntry = await ledgerEntryRepository.GetByIdAsync(
                existingRecord.LedgerEntryId,
                cancellationToken);

            if (originalLedgerEntry is null)
            {
                throw new InvalidOperationException("O lancamento associado a chave de idempotencia nao foi encontrado.");
            }

            return ToResult(originalLedgerEntry);
        }

        var ledgerEntry = ledgerEntryService.Create(
            command.MerchantId,
            command.EntryType,
            command.Amount,
            command.BusinessDate,
            command.Description);

        var idempotencyRecord = IdempotencyRecord.Create(
            command.MerchantId,
            command.IdempotencyKey,
            requestFingerprint,
            ledgerEntry.Id);
        var eventId = Guid.NewGuid();
        var integrationEvent = new LedgerEntryRegistered(
            eventId,
            ledgerEntry.Id,
            ledgerEntry.MerchantId,
            ledgerEntry.EntryType.ToString(),
            ledgerEntry.Amount.Amount,
            ledgerEntry.BusinessDate,
            ledgerEntry.RegisteredAtUtc);
        var integrationMessage = IntegrationMessage.Create(
            eventId,
            nameof(LedgerEntryRegistered),
            JsonSerializer.Serialize(integrationEvent),
            integrationEvent.OccurredAtUtc);

        await ledgerEntryRepository.AddAsync(ledgerEntry, cancellationToken);
        await idempotencyRecordRepository.AddAsync(idempotencyRecord, cancellationToken);
        await integrationMessageRepository.AddAsync(integrationMessage, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(ledgerEntry);
    }

    private static RegisterLedgerEntryResult ToResult(LedgerEntry ledgerEntry) => new(
            ledgerEntry.Id,
            ledgerEntry.EntryType,
            ledgerEntry.Amount.Amount,
            ledgerEntry.BusinessDate,
            ledgerEntry.Description,
            ledgerEntry.RegisteredAtUtc);
}
