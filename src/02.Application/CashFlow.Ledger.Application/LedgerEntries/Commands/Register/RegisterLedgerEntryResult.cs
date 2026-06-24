using CashFlow.Ledger.Domain.Enums;

namespace CashFlow.Ledger.Application.LedgerEntries.Commands.Register;

public sealed record RegisterLedgerEntryResult(
    Guid Id,
    EntryType EntryType,
    decimal Amount,
    DateOnly BusinessDate,
    string? Description,
    DateTimeOffset RegisteredAtUtc);
