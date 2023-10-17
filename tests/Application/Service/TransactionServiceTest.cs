using Xunit;
using Moq;
using Domain.Repository;
using Domain.Document;
using Application.Service;
using Infrastructure.Service;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace tests.Application.Service
{
    public class TransactionServiceTest
    {
        [Fact]
        public async Task SaveTransaction_SendsMessageAndSetsEmittedFlag_EnsureSendToKafka()
        {
            var transaction = new Transaction("123abcabc", 100.10m, DateTime.Now);
            var eventName = "transaction.created";
            var message = new Message<string, Transaction>{Key = "234abcabc", Value = transaction};

            var repositoryMock = new Mock<IRepository<Transaction>>();
            var producerMock = new Mock<IProducer<string, Transaction>>();
            var deliveryResult = new DeliveryResult<string, Transaction>() {Status = PersistenceStatus.Persisted, Message = message };

            repositoryMock.Setup(repo => repo.Save(transaction)).ReturnsAsync((Transaction p) => {
                transaction.Id = "234abcabc";

                return transaction;
            });
            
            repositoryMock.Setup(repo => repo.Update(transaction)).ReturnsAsync(transaction);
            producerMock.Setup(
                p => p.ProduceAsync(eventName, It.IsAny<Message<string, Transaction>>(), default)
            ).ReturnsAsync(deliveryResult);

            var messagingService = new DocumentMessagingService<Transaction>(
                producerMock.Object,
                new Mock<ILogger<DocumentMessagingService<Transaction>>>().Object
            );

            var transactionService = new TransactionService(repositoryMock.Object, messagingService, eventName);

            var result = await transactionService.Save(transaction);

            repositoryMock.Verify(repo => repo.Save(transaction), Times.Once);
            repositoryMock.Verify(repo => repo.Update(transaction), Times.Once);
            producerMock.Verify(ms => ms.ProduceAsync(
                eventName,
                It.IsAny<Message<string, Transaction>>(),
                default
            ), Times.Once);
            
            Assert.True(result.Emitted);
        }
    }
}