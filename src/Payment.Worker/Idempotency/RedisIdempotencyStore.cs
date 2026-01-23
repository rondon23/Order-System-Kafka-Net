using EventBus.Idempotency;
using StackExchange.Redis;

namespace Payment.Worker.Idempotency;

public sealed class RedisIdempotencyStore : IIdempotencyStore
{
    private readonly IDatabase _db;

    public RedisIdempotencyStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<bool> HasProcessedAsync(Guid eventId)
    {
        return await _db.KeyExistsAsync(Key(eventId));
    }

    public async Task MarkAsProcessedAsync(Guid eventId)
    {
        await _db.StringSetAsync(
            Key(eventId),
            "1",
            TimeSpan.FromDays(7)); // TTL
    }

    private static string Key(Guid eventId)
        => $"processed-event:{eventId}";
}