using System.Text.Json.Serialization;

namespace CashFlow.Balance.Api.Responses;

public sealed record DailyBalanceResponse(
    [property: JsonPropertyName("data")] DateOnly BusinessDate,
    [property: JsonPropertyName("totalCreditos")] decimal TotalCredits,
    [property: JsonPropertyName("totalDebitos")] decimal TotalDebits,
    [property: JsonPropertyName("saldo")] decimal Balance,
    [property: JsonPropertyName("ultimaAtualizacaoUtc")] DateTimeOffset LastUpdatedAtUtc);
