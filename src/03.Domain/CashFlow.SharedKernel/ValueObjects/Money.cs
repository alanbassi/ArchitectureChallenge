namespace CashFlow.SharedKernel.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Create(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "O valor deve ser maior que zero.");
        }

        return new Money(decimal.Round(amount, 2, MidpointRounding.ToEven));
    }
}
