using System.Text.Json.Serialization;

namespace CashFlow.SharedKernel.IntegrationEvents;

public sealed record LedgerEntryRegistered(
    [property: JsonPropertyName("eventId")] Guid EventId,
    [property: JsonPropertyName("ledgerEntryId")] Guid LedgerEntryId,
    [property: JsonPropertyName("merchantId")] string MerchantId,
    [property: JsonPropertyName("entryType")] string EntryType,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("businessDate")] DateOnly BusinessDate,
    [property: JsonPropertyName("occurredAtUtc")] DateTimeOffset OccurredAtUtc,
    [property: JsonPropertyName("contractVersion")] int ContractVersion = 1);
