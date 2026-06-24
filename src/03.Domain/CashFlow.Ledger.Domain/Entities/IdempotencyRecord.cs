using CashFlow.SharedKernel.Guards;

namespace CashFlow.Ledger.Domain.Entities;

public sealed class IdempotencyRecord
{
    private IdempotencyRecord()
    {
        MerchantId = null!;
        Key = null!;
        RequestFingerprint = null!;
    }

    private IdempotencyRecord(
        Guid id,
        string merchantId,
        string key,
        string requestFingerprint,
        Guid ledgerEntryId,
        DateTimeOffset registeredAtUtc)
    {
        Id = id;
        MerchantId = merchantId;
        Key = key;
        RequestFingerprint = requestFingerprint;
        LedgerEntryId = ledgerEntryId;
        RegisteredAtUtc = registeredAtUtc;
    }

    public Guid Id { get; private set; }

    public string MerchantId { get; private set; }

    public string Key { get; private set; }

    public string RequestFingerprint { get; private set; }

    public Guid LedgerEntryId { get; private set; }

    public DateTimeOffset RegisteredAtUtc { get; private set; }

    public static IdempotencyRecord Create(
        string merchantId,
        string key,
        string requestFingerprint,
        Guid ledgerEntryId,
        DateTimeOffset? registeredAtUtc = null)
    {
        Guard.NotEmpty(merchantId, nameof(merchantId));
        Guard.NotEmpty(key, nameof(key));
        Guard.MaxLength(key, 128, nameof(key));
        Guard.NotEmpty(requestFingerprint, nameof(requestFingerprint));
        Guard.NotEmpty(ledgerEntryId, nameof(ledgerEntryId));

        var registrationTime = registeredAtUtc ?? DateTimeOffset.UtcNow;
        Guard.MustBeUtc(registrationTime, nameof(registeredAtUtc));

        return new IdempotencyRecord(
            Guid.NewGuid(),
            merchantId.Trim(),
            key.Trim(),
            requestFingerprint,
            ledgerEntryId,
            registrationTime);
    }
}
