namespace CashFlow.Balance.Application.DailyBalances.Queries.Get;

public sealed record DailyBalanceQueryResult(
    DateOnly BusinessDate,
    decimal TotalCredits,
    decimal TotalDebits,
    decimal Balance,
    DateTimeOffset LastUpdatedAtUtc);
