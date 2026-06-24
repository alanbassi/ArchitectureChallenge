using CashFlow.SharedKernel.Guards;

namespace CashFlow.Balance.Domain.Entities;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    private InboxMessage(Guid eventId, DateTimeOffset processedAtUtc)
    {
        EventId = eventId;
        ProcessedAtUtc = processedAtUtc;
    }

    public Guid EventId { get; private set; }

    public DateTimeOffset ProcessedAtUtc { get; private set; }

    public static InboxMessage Create(Guid eventId, DateTimeOffset processedAtUtc)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("The event identifier is required.", nameof(eventId));
        }

        Guard.MustBeUtc(processedAtUtc, nameof(processedAtUtc));

        return new InboxMessage(eventId, processedAtUtc);
    }
}
