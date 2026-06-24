using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Repositories;
using Mediator;

namespace CashFlow.Ledger.Application.LedgerEntries.Queries.List;

public sealed class ListLedgerEntriesQueryHandler(ILedgerEntryRepository repository)
    : IQueryHandler<ListLedgerEntriesQuery, IReadOnlyList<LedgerEntry>>
{
    public ValueTask<IReadOnlyList<LedgerEntry>> Handle(
        ListLedgerEntriesQuery query,
        CancellationToken cancellationToken) =>
        new(repository.GetByMerchantAsync(query.MerchantId, cancellationToken));
}
