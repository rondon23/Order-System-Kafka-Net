using System.Text.Json;
using Confluent.Kafka;
using EventBus.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Payment.Worker.Kafka;

public sealed class OrderCreatedConsumer : IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly string _topic;

    public OrderCreatedConsumer(
        IConfiguration configuration,
        ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
        _topic = configuration["Kafka:OrderCreatedTopic"]!;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(_topic);
    }

    public void Consume(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);

                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                    result.Message.Value);

                _logger.LogInformation(
                    "Processing payment for OrderId {OrderId} | Amount {Amount}",
                    orderEvent!.OrderId,
                    orderEvent.Amount
                );

                // Simula processamento
                Thread.Sleep(1000);

                // Commit manual
                _consumer.Commit(result);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Payment.Worker is shutting down.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming OrderCreated event");
        }
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}
