using CashFlow.Balance.Domain.Repositories;
using CashFlow.Balance.Infrastructure.Persistence;

namespace CashFlow.Balance.Infrastructure.Repositories;

public sealed class EfBalanceUnitOfWork(BalanceDbContext dbContext) : IBalanceUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
