using Application.Service;
using Confluent.Kafka;
using Domain.Document;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service
{
    public class DocumentMessagingService<T> : IMessagingService<T> where T : IDocument
    {
        private IProducer<string, T> Producer;

        private ILogger Logger;

        public DocumentMessagingService(IProducer<string, T> producer, ILogger<DocumentMessagingService<T>> logger)
        {
            Producer = producer;
            Logger = logger;
        }

        public async Task SendMessage(string eventName, T data)
        {
            if (data.Id == null) {
                throw new InvalidDataException("please, dispatch only persisted data");
            }

            var result = await Producer.ProduceAsync(eventName, new Message<string, T> {Key = data.Id.ToString(), Value = data});

            Logger.LogInformation("Message {id} sent to {eventName}, status {status}", data.Id, eventName, result.Status);
        }
    }
}