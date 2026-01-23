using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;

namespace Payment.Worker.Kafka;

public sealed class KafkaDlqProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaDlqProducer(IConfiguration config)
    {
        _topic = config["Kafka:OrderCreatedDlqTopic"]!;
        _producer = new ProducerBuilder<string, string>(
            new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            }).Build();
    }

    public async Task SendAsync(OrderCreatedEvent evt)
    {
        await _producer.ProduceAsync(_topic,
            new Message<string, string>
            {
                Key = evt.EventId.ToString(),
                Value = JsonSerializer.Serialize(evt)
            });
    }
}