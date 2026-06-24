using CashFlow.Ledger.Application.Messaging;
using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashFlow.Ledger.Infrastructure.BackgroundServices;

public sealed class IntegrationMessagePublisherService(
    IServiceScopeFactory serviceScopeFactory,
    LedgerMessagingOptions options,
    ILogger<IntegrationMessagePublisherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishPendingMessagesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(options.PollingIntervalSeconds), stoppingToken);
        }
    }

    private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IIntegrationMessageRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<ILedgerUnitOfWork>();
        var messages = await repository.GetPendingAsync(options.BatchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await publisher.PublishAsync(message, cancellationToken);
                message.MarkAsPublished(DateTimeOffset.UtcNow);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to publish integration message {IntegrationMessageId}.",
                    message.Id);
            }
        }
    }
}
