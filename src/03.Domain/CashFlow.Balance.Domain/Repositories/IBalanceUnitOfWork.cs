namespace CashFlow.Balance.Domain.Repositories;

public interface IBalanceUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
