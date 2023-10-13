using Domain.Document;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class DailyCashFlowRepsitory : BaseRepository<DailyCashFlow>
    {
        public DailyCashFlowRepsitory(IMongoCollection<DailyCashFlow> mongoCollection) : base(mongoCollection)
        {
        }
    }
}