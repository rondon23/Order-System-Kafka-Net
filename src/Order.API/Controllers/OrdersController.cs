using EventBus.Events;
using Microsoft.AspNetCore.Mvc;
using Order.API.Kafka;

namespace Order.API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly KafkaProducer _producer;

    public OrdersController(KafkaProducer producer)
    {
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid(),
            CustomerId = "CUST-123",
            Amount = 299.90m
        };

        await _producer.PublishOrderCreatedAsync(orderEvent);

        return Accepted(new
        {
            orderEvent.OrderId,
            Message = "Order created and event published"
        });
    }
}