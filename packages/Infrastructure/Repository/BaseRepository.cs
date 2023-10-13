using System.Linq.Expressions;
using Domain.Document;
using Domain.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : IDocument
    {
        protected readonly IMongoCollection<T> MongoCollection;

        public BaseRepository(IMongoCollection<T> mongoCollection)
        {
            MongoCollection = mongoCollection;
        }

        public virtual async Task<List<T>> Find(Expression<Func<T, bool>> filter, string orderBy, int page = 1, int limit = 30)
        {
            return await MongoCollection.Find(filter)
                .Skip((page - 1) * limit)
                .Limit(limit)
                .Sort(Builders<T>.Sort.Ascending(orderBy))
                .ToListAsync();
        }

        public virtual async Task<T?> FindById(string id)
        {
            return await MongoCollection.Find(d => d.Id == id).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<T?> FindOne(Expression<Func<T, bool>> filter)
        {
            return await MongoCollection.Find(filter).Limit(1).FirstOrDefaultAsync();
        }

        public virtual async Task<T> Save(T document)
        {
            await MongoCollection.InsertOneAsync(document);

            return document;
        }

        public virtual async Task<T> Update(T document)
        {
            return await MongoCollection.FindOneAndReplaceAsync(d => d.Id == document.Id, document);
        }
    }
}