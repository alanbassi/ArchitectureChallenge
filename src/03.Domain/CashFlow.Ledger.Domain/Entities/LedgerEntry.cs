using CashFlow.Ledger.Domain.Enums;
using CashFlow.SharedKernel.ValueObjects;

namespace CashFlow.Ledger.Domain.Entities;

public sealed class LedgerEntry
{
    private LedgerEntry()
    {
        MerchantId = null!;
    }

    internal LedgerEntry(
        Guid id,
        string merchantId,
        EntryType entryType,
        Money amount,
        DateOnly businessDate,
        string? description,
        DateTimeOffset registeredAtUtc)
    {
        Id = id;
        MerchantId = merchantId;
        EntryType = entryType;
        Amount = amount;
        BusinessDate = businessDate;
        Description = description;
        RegisteredAtUtc = registeredAtUtc;
    }

    public Guid Id { get; private set; }

    public string MerchantId { get; private set; }

    public EntryType EntryType { get; private set; }

    public Money Amount { get; private set; }

    public DateOnly BusinessDate { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset RegisteredAtUtc { get; private set; }
}
