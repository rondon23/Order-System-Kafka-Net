using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using Payment.Worker.Persistence;

namespace Payment.Worker.Kafka;

public sealed class OrderCreatedConsumer : IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly KafkaRetryProducer _retryProducer;
    private readonly ProcessedEventStore _eventStore;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly int _maxRetry;

    public OrderCreatedConsumer(
        IConfiguration configuration,
        KafkaRetryProducer retryProducer,
        ProcessedEventStore eventStore,
        ILogger<OrderCreatedConsumer> logger)
    {
        _retryProducer = retryProducer;
        _eventStore = eventStore;
        _logger = logger;

        _maxRetry = int.Parse(configuration["Kafka:MaxRetryCount"]!);

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(configuration["Kafka:OrderCreatedTopic"]!);
    }

    public void Consume(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                result.Message.Value)!;

            // 🧠 IDEMPOTÊNCIA
            if (_eventStore.HasBeenProcessed(orderEvent.EventId))
            {
                _logger.LogWarning(
                    "Event {EventId} already processed. Skipping.",
                    orderEvent.EventId);

                _consumer.Commit(result);
                continue;
            }

            try
            {
                ProcessPayment(orderEvent);

                _eventStore.MarkAsProcessed(orderEvent.EventId);
                _consumer.Commit(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing OrderId {OrderId}",
                    orderEvent.OrderId);

                HandleRetry(orderEvent, result);
            }
        }
    }

    private void ProcessPayment(OrderCreatedEvent orderEvent)
    {
        // Simula falha aleatória
        if (Random.Shared.Next(1, 4) == 1)
            throw new Exception("Payment gateway timeout");

        _logger.LogInformation(
            "Payment processed for OrderId {OrderId}",
            orderEvent.OrderId);
    }

    private async void HandleRetry(
        OrderCreatedEvent orderEvent,
        ConsumeResult<string, string> result)
    {
        var retryCount = result.Message.Headers
            .TryGetLastBytes("retry-count", out var value)
            ? int.Parse(System.Text.Encoding.UTF8.GetString(value))
            : 0;

        if (retryCount < _maxRetry)
        {
            await _retryProducer.SendToRetryAsync(orderEvent);

            _logger.LogWarning(
                "Event {EventId} sent to retry ({Retry})",
                orderEvent.EventId,
                retryCount + 1);
        }
        else
        {
            await _retryProducer.SendToDlqAsync(orderEvent);

            _logger.LogError(
                "Event {EventId} sent to DLQ",
                orderEvent.EventId);
        }

        _consumer.Commit(result);
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}
    