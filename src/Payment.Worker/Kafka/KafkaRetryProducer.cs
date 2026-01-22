using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;

namespace Payment.Worker.Kafka;

public sealed class KafkaRetryProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly IConfiguration _configuration;

    public KafkaRetryProducer(IConfiguration configuration)
    {
        _configuration = configuration;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendToRetryAsync(OrderCreatedEvent @event)
    {
        await SendAsync(
            _configuration["Kafka:OrderCreatedRetryTopic"]!,
            @event);
    }

    public async Task SendToDlqAsync(OrderCreatedEvent @event)
    {
        await SendAsync(
            _configuration["Kafka:OrderCreatedDlqTopic"]!,
            @event);
    }

    private async Task SendAsync(string topic, OrderCreatedEvent @event)
    {
        var message = new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = JsonSerializer.Serialize(@event)
        };

        await _producer.ProduceAsync(topic, message);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}