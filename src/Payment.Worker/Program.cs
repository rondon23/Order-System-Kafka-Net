using Payment.Worker;
using Payment.Worker.Persistence;
using Payment.Worker.Kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ProcessedEventStore>();
        services.AddSingleton<KafkaRetryProducer>();
        services.AddSingleton<OrderCreatedConsumer>();
        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();