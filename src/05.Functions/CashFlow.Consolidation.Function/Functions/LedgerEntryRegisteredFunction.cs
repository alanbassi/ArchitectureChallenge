using CashFlow.Balance.Application.DailyBalances.Commands.ProcessLedgerEntryRegistered;
using CashFlow.SharedKernel.IntegrationEvents;
using Mediator;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace CashFlow.Consolidation.Function.Functions;

public sealed class LedgerEntryRegisteredFunction(IMediator mediator)
{
    [Function(nameof(LedgerEntryRegisteredFunction))]
    public async Task Run(
        [ServiceBusTrigger("%ServiceBusEntityName%", Connection = "ServiceBusConnection")]
        string messageBody,
        CancellationToken cancellationToken)
    {
        var integrationEvent = JsonSerializer.Deserialize<LedgerEntryRegistered>(messageBody)
            ?? throw new InvalidOperationException("The LedgerEntryRegistered message body is invalid.");

        var command = new ProcessLedgerEntryRegisteredCommand(
            integrationEvent.EventId,
            integrationEvent.LedgerEntryId,
            integrationEvent.MerchantId,
            integrationEvent.EntryType,
            integrationEvent.Amount,
            integrationEvent.BusinessDate,
            integrationEvent.OccurredAtUtc,
            integrationEvent.ContractVersion);

        await mediator.Send(command, cancellationToken);
    }
}
