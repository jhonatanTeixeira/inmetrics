using Domain.Document;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IMongoCollection<User> mongoCollection) : base(mongoCollection)
        {
        }
    }
}