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
        Logger.LogInformation("Consumer stating");
        Consumer.Subscribe("DAILY_CASH_FLOW_TABLE");
        Logger.LogInformation("Consumer subscribed");

        while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("waiting for the next message");
            var message = Consumer.Consume(stoppingToken).Message;

            Logger.LogInformation("Received Daily cash flow for Id {U} - {D}", message.Key.UserId, message.Key.Date);

            var document = await Service.Save(new DailyCashFlow(message.Key.UserId, message.Key.Date, message.Value.Total));

            Consumer.Commit();

            Logger.LogInformation("Daily cash flow id {U} - {D} persisted Amount {A}", message.Key.UserId, message.Key.Date, document.Amount);

            await Task.Delay(100, stoppingToken);
        }
    }
}
