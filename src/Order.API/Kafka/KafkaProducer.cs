using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;

namespace Order.API.Kafka;

public sealed class KafkaProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _orderCreatedTopic;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _orderCreatedTopic = configuration["Kafka:OrderCreatedTopic"]!;
    }

    public async Task PublishOrderCreatedAsync(OrderCreatedEvent @event)
    {
        var message = new Message<string, string>
        {
            Key = @event.OrderId.ToString(),
            Value = JsonSerializer.Serialize(@event)
        };

        await _producer.ProduceAsync(_orderCreatedTopic, message);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}