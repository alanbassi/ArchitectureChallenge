using CashFlow.Balance.Domain.Entities;

namespace CashFlow.Balance.Domain.Repositories;

public interface IInboxMessageRepository
{
    Task<InboxMessage?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);

    Task AddAsync(InboxMessage inboxMessage, CancellationToken cancellationToken);
}
