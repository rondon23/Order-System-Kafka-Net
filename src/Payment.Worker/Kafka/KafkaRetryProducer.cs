using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;
using Payment.Worker.Retry;

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

    /// <summary>
    /// Envia o evento para o tópico de retry aplicando exponential backoff
    /// </summary>
    public async Task SendToRetryAsync(
        OrderCreatedEvent @event,
        int retryCount)
    {
        var delay = RetryPolicy.CalculateDelay(retryCount);
        var retryAt = DateTime.UtcNow.Add(delay);

        var message = new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = JsonSerializer.Serialize(@event),
            Headers = new Headers
            {
                { "retry-count", Encoding.UTF8.GetBytes(retryCount.ToString()) },
                { "retry-at", Encoding.UTF8.GetBytes(retryAt.ToString("O")) }
            }
        };

        await _producer.ProduceAsync(
            _configuration["Kafka:OrderCreatedRetryTopic"]!,
            message);
    }

    /// <summary>
    /// Envia o evento definitivamente para a Dead Letter Queue
    /// </summary>
    public async Task SendToDlqAsync(OrderCreatedEvent @event)
    {
        var message = new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = JsonSerializer.Serialize(@event)
        };

        await _producer.ProduceAsync(
            _configuration["Kafka:OrderCreatedDlqTopic"]!,
            message);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
