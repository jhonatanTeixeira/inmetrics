using Application.Service;
using Confluent.Kafka;
using DailyCashFlowConsumer.Dto;
using Domain.Document;

namespace DailyCashFlowConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> Logger;

    private readonly IConsumer<CashFlowKey, CashFlowValue> Consumer;

    private readonly IUserResourceCrudService<DailyCashFlow> Service;

    public Worker(ILogger<Worker> logger, IConsumer<CashFlowKey, CashFlowValue> consumer, IUserResourceCrudService<DailyCashFlow> service)
    {
        Logger = logger;
        Consumer = consumer;
        Service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Consumer.Subscribe("DAILY_CASH_FLOW_TABLE");

            var result = Consumer.Consume(stoppingToken);

            Logger.LogInformation("Received Daily cash flow for Id {O}", result.Key);

            var document = await Service.Save(new DailyCashFlow(result.Key.UserId, result.Key.Date, result.Value.Total));

            Logger.LogInformation("Daily cash flow id {ID} persisted Amount {A}", result.Key, document.Amount);

            await Task.Delay(100, stoppingToken);
        }
    }
}
