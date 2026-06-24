using Mediator;

namespace CashFlow.Balance.Application.DailyBalances.Queries.Get;

public sealed record GetDailyBalanceQuery(string MerchantId, DateOnly BusinessDate)
    : IQuery<DailyBalanceQueryResult?>;
