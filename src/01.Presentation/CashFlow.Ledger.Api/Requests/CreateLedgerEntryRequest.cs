using System.Text.Json.Serialization;

namespace CashFlow.Ledger.Api.Requests;

public sealed record CreateLedgerEntryRequest(
    [property: JsonPropertyName("comercianteId")] string MerchantId,
    [property: JsonPropertyName("tipo")] string Type,
    [property: JsonPropertyName("valor")] decimal Amount,
    [property: JsonPropertyName("data")] DateOnly BusinessDate,
    [property: JsonPropertyName("descricao")] string? Description);
