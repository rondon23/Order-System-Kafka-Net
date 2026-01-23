using EventBus.Idempotency;
using Payment.Worker;
using Payment.Worker.Persistence;
using Payment.Worker.Kafka;
using Payment.Worker.Consumers;
using Payment.Worker.Idempotency;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ProcessedEventStore>();
        services.AddSingleton<KafkaRetryProducer>();
        services.AddSingleton<OrderCreatedConsumer>();
        services.AddSingleton<IIdempotencyStore, RedisIdempotencyStore>();
        services.AddSingleton<KafkaDlqProducer>();
        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();