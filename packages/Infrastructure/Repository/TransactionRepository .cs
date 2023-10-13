using Domain.Document;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public TransactionRepository(IMongoCollection<Transaction> mongoCollection) : base(mongoCollection)
        {
        }
    }
}