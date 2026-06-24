using CashFlow.Ledger.Domain.Enums;
using Mediator;

namespace CashFlow.Ledger.Application.LedgerEntries.Commands.Register;

public sealed record RegisterLedgerEntryCommand(
    string MerchantId,
    EntryType EntryType,
    decimal Amount,
    DateOnly BusinessDate,
    string? Description,
    string IdempotencyKey) : ICommand<RegisterLedgerEntryResult>;
