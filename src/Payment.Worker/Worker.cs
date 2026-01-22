using Payment.Worker.Kafka;

namespace Payment.Worker;

public sealed class Worker : BackgroundService
{
    private readonly OrderCreatedConsumer _consumer;
    private readonly ILogger<Worker> _logger;

    public Worker(
        OrderCreatedConsumer consumer,
        ILogger<Worker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment.Worker started");
        _consumer.Consume(stoppingToken);
        return Task.CompletedTask;
    }
}