using CashFlow.Ledger.Domain.Entities;

namespace CashFlow.Ledger.Domain.Repositories;

public interface ILedgerEntryRepository
{
    Task<LedgerEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<LedgerEntry>> GetByMerchantAsync(
        string merchantId,
        CancellationToken cancellationToken);

    Task AddAsync(LedgerEntry ledgerEntry, CancellationToken cancellationToken);
}
