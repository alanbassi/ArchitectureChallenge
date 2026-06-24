namespace CashFlow.Ledger.Domain.Repositories;

public interface ILedgerUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
