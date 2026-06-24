using CashFlow.Balance.Api.Responses;
using CashFlow.Balance.Application.DailyBalances.Queries.Get;

namespace CashFlow.Balance.Api.Mappings;

public static class DailyBalanceMappings
{
    public static DailyBalanceResponse ToResponse(this DailyBalanceQueryResult result) =>
        new(
            result.BusinessDate,
            result.TotalCredits,
            result.TotalDebits,
            result.Balance,
            result.LastUpdatedAtUtc);
}
