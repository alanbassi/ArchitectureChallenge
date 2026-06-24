using CashFlow.Balance.Domain.Entities;
using CashFlow.Balance.Domain.Repositories;
using CashFlow.Balance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Balance.Infrastructure.Repositories;

public sealed class EfInboxMessageRepository(BalanceDbContext dbContext) : IInboxMessageRepository
{
    public Task<InboxMessage?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        dbContext.InboxMessages.FirstOrDefaultAsync(message => message.EventId == eventId, cancellationToken);

    public async Task AddAsync(InboxMessage inboxMessage, CancellationToken cancellationToken)
    {
        await dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
    }
}
