using Confluent.Kafka;
using DailyCashFlowConsumer.Dto;

namespace DailyCashFlowConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConsumer<CashFlowKey, CashFlowValue> Consumer;

    public Worker(ILogger<Worker> logger, IConsumer<CashFlowKey, CashFlowValue> consumer)
    {
        _logger = logger;
        Consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Consumer.Subscribe("DAILY_CASH_FLOW_TABLE");

            var result = Consumer.Consume(stoppingToken);

            Console.WriteLine(result.Message.Value);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
