using System.Text.Json.Serialization;

namespace CashFlow.Ledger.Api.Responses;

public sealed record LedgerEntryResponse(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("tipo")] string Type,
    [property: JsonPropertyName("valor")] decimal Amount,
    [property: JsonPropertyName("data")] DateOnly BusinessDate,
    [property: JsonPropertyName("descricao")] string? Description,
    [property: JsonPropertyName("registradoEmUtc")] DateTimeOffset RegisteredAtUtc);
