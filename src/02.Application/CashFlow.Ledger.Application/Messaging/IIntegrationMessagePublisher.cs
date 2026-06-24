using CashFlow.Ledger.Domain.Entities;

namespace CashFlow.Ledger.Application.Messaging;

public interface IIntegrationMessagePublisher
{
    Task PublishAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken);
}
