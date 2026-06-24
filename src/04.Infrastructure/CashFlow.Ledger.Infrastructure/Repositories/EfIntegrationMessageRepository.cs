using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Ledger.Infrastructure.Repositories;

public sealed class EfIntegrationMessageRepository(LedgerDbContext dbContext) : IIntegrationMessageRepository
{
    public async Task<IReadOnlyList<IntegrationMessage>> GetPendingAsync(
        int maximumCount,
        CancellationToken cancellationToken)
    {
        return await dbContext.IntegrationMessages
            .Where(message => message.PublishedAtUtc == null)
            .OrderBy(message => message.OccurredAtUtc)
            .Take(maximumCount)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken)
    {
        await dbContext.IntegrationMessages.AddAsync(integrationMessage, cancellationToken);
    }
}
