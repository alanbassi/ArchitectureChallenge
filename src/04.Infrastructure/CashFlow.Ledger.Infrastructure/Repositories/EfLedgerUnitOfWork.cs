using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Persistence;

namespace CashFlow.Ledger.Infrastructure.Repositories;

public sealed class EfLedgerUnitOfWork(LedgerDbContext dbContext) : ILedgerUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
