namespace Payment.Worker.Persistence;

public sealed class ProcessedEventStore
{
    private readonly HashSet<Guid> _processedEvents = new();

    public bool HasBeenProcessed(Guid eventId)
    {
        return _processedEvents.Contains(eventId);
    }

    public void MarkAsProcessed(Guid eventId)
    {
        _processedEvents.Add(eventId);
    }
}