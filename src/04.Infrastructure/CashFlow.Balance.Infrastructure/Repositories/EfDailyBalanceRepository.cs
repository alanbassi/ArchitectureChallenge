using CashFlow.Balance.Domain.Entities;
using CashFlow.Balance.Domain.Repositories;
using CashFlow.Balance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Balance.Infrastructure.Repositories;

public sealed class EfDailyBalanceRepository(BalanceDbContext dbContext) : IDailyBalanceRepository
{
    public Task<DailyBalance?> GetByMerchantAndBusinessDateAsync(
        string merchantId,
        DateOnly businessDate,
        CancellationToken cancellationToken) =>
        dbContext.DailyBalances.FirstOrDefaultAsync(
            balance => balance.MerchantId == merchantId.Trim() && balance.BusinessDate == businessDate,
            cancellationToken);

    public async Task AddAsync(DailyBalance dailyBalance, CancellationToken cancellationToken)
    {
        await dbContext.DailyBalances.AddAsync(dailyBalance, cancellationToken);
    }
}
