using System.Linq.Expressions;
using Domain.Document;
using Domain.Repository;

namespace Application.Service
{
    public class BaseCrudService<T> : ICrudService<T> where T : IDocument
    {
        protected readonly IRepository<T> Repository;

        public BaseCrudService(IRepository<T> repository)
        {
            Repository = repository;
        }

        public virtual async Task<T?> GetById(string id)
        {
            return await Repository.FindById(id);
        }

        public virtual async Task<List<T>> GetList(Expression<Func<T, bool>> filter, string orderBy, int page = 1, int limit = 30)
        {
            return await Repository.Find(filter, orderBy, page, limit);
        }

        public virtual async Task<T> Save(T document)
        {
            if (document.Id == null) {
                return await Repository.Save(document);
            }

            return await Repository.Update(document);
        }
    }
}