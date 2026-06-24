using CashFlow.SharedKernel.Guards;

namespace CashFlow.Balance.Domain.Entities;

public sealed class DailyBalance
{
    private DailyBalance()
    {
        MerchantId = null!;
    }

    private DailyBalance(string merchantId, DateOnly businessDate, DateTimeOffset updatedAtUtc)
    {
        MerchantId = merchantId;
        BusinessDate = businessDate;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string MerchantId { get; private set; }

    public DateOnly BusinessDate { get; private set; }

    public decimal TotalCredits { get; private set; }

    public decimal TotalDebits { get; private set; }

    public decimal Balance { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static DailyBalance Create(string merchantId, DateOnly businessDate, DateTimeOffset updatedAtUtc)
    {
        Guard.NotEmpty(merchantId, nameof(merchantId));
        Guard.NotDefault(businessDate, nameof(businessDate));
        Guard.MustBeUtc(updatedAtUtc, nameof(updatedAtUtc));

        return new DailyBalance(merchantId.Trim(), businessDate, updatedAtUtc);
    }

    public void AddCredit(decimal amount, DateTimeOffset updatedAtUtc)
    {
        EnsurePositiveAmount(amount);
        Guard.MustBeUtc(updatedAtUtc, nameof(updatedAtUtc));

        TotalCredits += amount;
        Balance += amount;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void AddDebit(decimal amount, DateTimeOffset updatedAtUtc)
    {
        EnsurePositiveAmount(amount);
        Guard.MustBeUtc(updatedAtUtc, nameof(updatedAtUtc));

        TotalDebits += amount;
        Balance -= amount;
        UpdatedAtUtc = updatedAtUtc;
    }

    private static void EnsurePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero.");
        }
    }
}
