using System.Linq.Expressions;
using Domain.Document;

namespace Application.Service
{
    public interface ICrudService<T> where T : IDocument
    {
        public Task<List<T>> GetList(Expression<Func<T, bool>> filter, string orderBy, int page = 1, int limit = 30);

        public Task<T?> GetById(string id);

        public Task<T> Save(T document);
    }
}