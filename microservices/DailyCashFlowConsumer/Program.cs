using Confluent.Kafka;
using DailyCashFlowConsumer;
using DailyCashFlowConsumer.Dto;
using Infrastructure.Extension;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddDailyCashFlow();
        services.AddLogging();

        services.Configure<ConsumerConfig>(context.Configuration.GetSection("KafkaConsumer"));
        services.AddKafkaConsumer<CashFlowKey, CashFlowValue>();
    })
    .Build();

await host.RunAsync();
