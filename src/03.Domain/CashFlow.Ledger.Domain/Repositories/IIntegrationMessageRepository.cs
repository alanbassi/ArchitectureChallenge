using CashFlow.Ledger.Domain.Entities;

namespace CashFlow.Ledger.Domain.Repositories;

public interface IIntegrationMessageRepository
{
    Task<IReadOnlyList<IntegrationMessage>> GetPendingAsync(
        int maximumCount,
        CancellationToken cancellationToken);

    Task AddAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken);
}
