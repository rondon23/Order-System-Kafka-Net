using Payment.Worker;
using Payment.Worker.Kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<OrderCreatedConsumer>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();