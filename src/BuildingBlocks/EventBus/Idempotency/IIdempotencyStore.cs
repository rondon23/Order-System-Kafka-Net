namespace EventBus.Idempotency;

public interface IIdempotencyStore
{
    Task<bool> HasProcessedAsync(Guid eventId);
    Task MarkAsProcessedAsync(Guid eventId);
}