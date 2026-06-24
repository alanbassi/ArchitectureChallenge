using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Ledger.Infrastructure.Repositories;

public sealed class EfLedgerEntryRepository(LedgerDbContext dbContext) : ILedgerEntryRepository
{
    public Task<LedgerEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.LedgerEntries.FirstOrDefaultAsync(entry => entry.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LedgerEntry>> GetByMerchantAsync(
        string merchantId,
        CancellationToken cancellationToken) =>
        await dbContext.LedgerEntries
            .Where(entry => entry.MerchantId == merchantId.Trim())
            .OrderByDescending(entry => entry.RegisteredAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(LedgerEntry ledgerEntry, CancellationToken cancellationToken)
    {
        await dbContext.LedgerEntries.AddAsync(ledgerEntry, cancellationToken);
    }
}
