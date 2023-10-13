using Domain.Document;
using Domain.Repository;

namespace Application.Service
{
    public class TransactionService : BaseUserResourceCrudService<Transaction>
    {
        private readonly IMessagingService<Transaction> MessagingService;

        private readonly string EventName;

        public TransactionService(IRepository<Transaction> repository, IMessagingService<Transaction> messagingService, string eventName) : base(repository)
        {
            MessagingService = messagingService;
            EventName = eventName;
        }

        public override async Task<Transaction> Save(Transaction transaction)
        {
            transaction = await base.Save(transaction);

            await MessagingService.SendMessage(EventName, transaction);

            transaction.Emitted = true;

            return await Repository.Update(transaction);
        }
    }
}