using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Enums;
using CashFlow.SharedKernel.Guards;
using CashFlow.SharedKernel.ValueObjects;

namespace CashFlow.Ledger.Domain.Services;

public sealed class LedgerEntryService
{
    public LedgerEntry Create(
        string merchantId,
        EntryType entryType,
        decimal amount,
        DateOnly businessDate,
        string? description,
        Guid? id = null,
        DateTimeOffset? registeredAtUtc = null)
    {
        Guard.NotEmpty(merchantId, nameof(merchantId));
        Guard.DefinedEnum(entryType, nameof(entryType));
        Guard.NotDefault(businessDate, nameof(businessDate));

        var registrationTime = registeredAtUtc ?? DateTimeOffset.UtcNow;
        Guard.MustBeUtc(registrationTime, nameof(registeredAtUtc));

        return new LedgerEntry(
            id ?? Guid.NewGuid(),
            merchantId.Trim(),
            entryType,
            Money.Create(amount),
            businessDate,
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            registrationTime);
    }
}
