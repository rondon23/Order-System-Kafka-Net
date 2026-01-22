namespace EventBus.Events;

public sealed class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public string CustomerId { get; init; } = default!;
    public decimal Amount { get; init; }
}