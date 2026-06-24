using CashFlow.Ledger.Domain.Entities;
using Mediator;

namespace CashFlow.Ledger.Application.LedgerEntries.Queries.List;

public sealed record ListLedgerEntriesQuery(string MerchantId)
    : IQuery<IReadOnlyList<LedgerEntry>>;
