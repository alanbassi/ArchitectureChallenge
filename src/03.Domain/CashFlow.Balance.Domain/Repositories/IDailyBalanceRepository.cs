using CashFlow.Balance.Domain.Entities;

namespace CashFlow.Balance.Domain.Repositories;

public interface IDailyBalanceRepository
{
    Task<DailyBalance?> GetByMerchantAndBusinessDateAsync(
        string merchantId,
        DateOnly businessDate,
        CancellationToken cancellationToken);

    Task AddAsync(DailyBalance dailyBalance, CancellationToken cancellationToken);
}
