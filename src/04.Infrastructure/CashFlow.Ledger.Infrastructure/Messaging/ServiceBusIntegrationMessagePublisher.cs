using Azure.Messaging.ServiceBus;
using CashFlow.Ledger.Application.Messaging;
using CashFlow.Ledger.Domain.Entities;

namespace CashFlow.Ledger.Infrastructure.Messaging;

public sealed class ServiceBusIntegrationMessagePublisher(ServiceBusSender sender) : IIntegrationMessagePublisher
{
    public Task PublishAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken)
    {
        var message = new ServiceBusMessage(BinaryData.FromString(integrationMessage.Payload))
        {
            MessageId = integrationMessage.Id.ToString(),
            Subject = integrationMessage.Type,
            ContentType = "application/json"
        };

        return sender.SendMessageAsync(message, cancellationToken);
    }
}
