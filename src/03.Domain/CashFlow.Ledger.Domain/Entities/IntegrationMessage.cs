using CashFlow.SharedKernel.Guards;

namespace CashFlow.Ledger.Domain.Entities;

public sealed class IntegrationMessage
{
    private IntegrationMessage()
    {
        Type = null!;
        Payload = null!;
    }

    private IntegrationMessage(
        Guid id,
        string type,
        string payload,
        DateTimeOffset occurredAtUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string Payload { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public static IntegrationMessage Create(
        Guid id,
        string type,
        string payload,
        DateTimeOffset occurredAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("The integration message identifier is required.", nameof(id));
        }

        Guard.NotEmpty(type, nameof(type));
        Guard.NotEmpty(payload, nameof(payload));
        Guard.MustBeUtc(occurredAtUtc, nameof(occurredAtUtc));

        return new IntegrationMessage(id, type.Trim(), payload, occurredAtUtc);
    }

    public void MarkAsPublished(DateTimeOffset publishedAtUtc)
    {
        Guard.MustBeUtc(publishedAtUtc, nameof(publishedAtUtc));

        PublishedAtUtc = publishedAtUtc;
    }
}
