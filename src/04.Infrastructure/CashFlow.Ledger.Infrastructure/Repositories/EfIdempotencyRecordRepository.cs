using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Ledger.Infrastructure.Repositories;

public sealed class EfIdempotencyRecordRepository(LedgerDbContext dbContext) : IIdempotencyRecordRepository
{
    public Task<IdempotencyRecord?> GetByMerchantAndKeyAsync(
        string merchantId,
        string key,
        CancellationToken cancellationToken) =>
        dbContext.IdempotencyRecords.FirstOrDefaultAsync(
            record => record.MerchantId == merchantId.Trim() && record.Key == key.Trim(),
            cancellationToken);

    public async Task AddAsync(IdempotencyRecord idempotencyRecord, CancellationToken cancellationToken)
    {
        await dbContext.IdempotencyRecords.AddAsync(idempotencyRecord, cancellationToken);
    }
}
