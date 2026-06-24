using CashFlow.Ledger.Domain.Entities;

namespace CashFlow.Ledger.Domain.Repositories;

public interface IIdempotencyRecordRepository
{
    Task<IdempotencyRecord?> GetByMerchantAndKeyAsync(
        string merchantId,
        string key,
        CancellationToken cancellationToken);

    Task AddAsync(IdempotencyRecord idempotencyRecord, CancellationToken cancellationToken);
}
