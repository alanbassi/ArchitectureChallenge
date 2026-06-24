using CashFlow.Balance.Domain.Entities;

namespace CashFlow.Balance.Domain.Services;

public sealed class DailyBalanceService
{
    public DailyBalance Create(string merchantId, DateOnly businessDate, DateTimeOffset updatedAtUtc) =>
        DailyBalance.Create(merchantId, businessDate, updatedAtUtc);

    public void Apply(
        DailyBalance dailyBalance,
        string entryType,
        decimal amount,
        DateTimeOffset updatedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(dailyBalance);

        switch (entryType.Trim().ToUpperInvariant())
        {
            case "CREDIT":
                dailyBalance.AddCredit(amount, updatedAtUtc);
                break;

            case "DEBIT":
                dailyBalance.AddDebit(amount, updatedAtUtc);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(entryType), "The entry type must be Credit or Debit.");
        }
    }
}
