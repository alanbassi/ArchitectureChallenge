using Azure.Messaging.ServiceBus;
using CashFlow.Ledger.Application.Messaging;
using CashFlow.Ledger.Infrastructure.BackgroundServices;
using CashFlow.Ledger.Infrastructure.Configuration;
using CashFlow.Ledger.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Ledger.Infrastructure.DependencyInjection;

public static class LedgerMessagingServiceCollectionExtensions
{
    public static IServiceCollection AddLedgerMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(LedgerMessagingOptions.SectionName)
            .Get<LedgerMessagingOptions>() ?? new LedgerMessagingOptions();

        services.AddSingleton(options);

        if (string.Equals(options.Mode, "Disabled", StringComparison.OrdinalIgnoreCase))
        {
            return services;
        }

        if (!string.Equals(options.Mode, "AzureServiceBus", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Invalid messaging mode: {options.Mode}.");
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException("A Service Bus connection string is required for AzureServiceBus mode.");
        }

        if (string.IsNullOrWhiteSpace(options.EntityName))
        {
            throw new InvalidOperationException("A Service Bus entity name is required for AzureServiceBus mode.");
        }

        if (options.BatchSize <= 0 || options.PollingIntervalSeconds <= 0)
        {
            throw new InvalidOperationException("Messaging batch size and polling interval must be greater than zero.");
        }

        services.AddSingleton(_ => new ServiceBusClient(
            options.ConnectionString,
            new ServiceBusClientOptions
            {
                RetryOptions = new ServiceBusRetryOptions
                {
                    MaxRetries = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(10)
                }
            }));
        services.AddSingleton(serviceProvider => serviceProvider
            .GetRequiredService<ServiceBusClient>()
            .CreateSender(options.EntityName));
        services.AddSingleton<IIntegrationMessagePublisher, ServiceBusIntegrationMessagePublisher>();
        services.AddHostedService<IntegrationMessagePublisherService>();

        return services;
    }
}
