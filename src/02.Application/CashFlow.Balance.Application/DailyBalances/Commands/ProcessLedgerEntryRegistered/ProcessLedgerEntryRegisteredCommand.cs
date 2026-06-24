using Mediator;

namespace CashFlow.Balance.Application.DailyBalances.Commands.ProcessLedgerEntryRegistered;

public sealed record ProcessLedgerEntryRegisteredCommand(
    Guid EventId,
    Guid LedgerEntryId,
    string MerchantId,
    string EntryType,
    decimal Amount,
    DateOnly BusinessDate,
    DateTimeOffset OccurredAtUtc,
    int ContractVersion) : ICommand<ProcessLedgerEntryRegisteredResult>;
