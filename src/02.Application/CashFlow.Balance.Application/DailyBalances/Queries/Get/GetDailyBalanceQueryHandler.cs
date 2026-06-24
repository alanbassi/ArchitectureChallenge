using CashFlow.Balance.Domain.Repositories;
using Mediator;

namespace CashFlow.Balance.Application.DailyBalances.Queries.Get;

public sealed class GetDailyBalanceQueryHandler(IDailyBalanceRepository repository)
    : IQueryHandler<GetDailyBalanceQuery, DailyBalanceQueryResult?>
{
    public async ValueTask<DailyBalanceQueryResult?> Handle(
        GetDailyBalanceQuery query,
        CancellationToken cancellationToken)
    {
        var balance = await repository.GetByMerchantAndBusinessDateAsync(
            query.MerchantId,
            query.BusinessDate,
            cancellationToken);

        return balance is null
            ? null
            : new DailyBalanceQueryResult(
                balance.BusinessDate,
                balance.TotalCredits,
                balance.TotalDebits,
                balance.Balance,
                balance.UpdatedAtUtc);
    }
}
