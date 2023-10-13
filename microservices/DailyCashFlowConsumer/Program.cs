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
        services.AddMongo(
            context.Configuration.GetValue<string>("MongoConnectionString"),
            context.Configuration.GetValue<string>("MongoDatabase")
        );
        services.AddKafkaConsumer<CashFlowKey, CashFlowValue>();
    })
    .Build();

await host.RunAsync();
