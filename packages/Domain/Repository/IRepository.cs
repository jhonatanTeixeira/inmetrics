
using System.Linq.Expressions;
using Domain.Document;

namespace Domain.Repository
{
    public interface IRepository<T> where T : IDocument
    {
        public Task<T?> FindById(string id);

        public Task<List<T>> Find(Expression<Func<T, bool>> filter, string orderBy, int page = 1, int limit = 30);
        
        public Task<T?> FindOne(Expression<Func<T, bool>> filter);

        public Task<T> Save(T document);

        public Task<T> Update(T document);
    }
}