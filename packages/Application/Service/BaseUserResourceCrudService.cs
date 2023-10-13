using System.Linq.Expressions;
using Domain.Document;
using Domain.Repository;

namespace Application.Service
{
    public class BaseUserResourceCrudService<T> : BaseCrudService<T>, IUserResourceCrudService<T> where T : IUserResource
    {
        public BaseUserResourceCrudService(IRepository<T> repository) : base(repository)
        {
        }

        public virtual async Task<List<T>> GetListForUser(string userId, Expression<Func<T, bool>>? filter = null, string orderBy = "Id", int page = 1, int limit = 30)
        {
            Expression<Func<T, bool>> expr = d => d.UserId == userId;

            if (filter != null) {
                expr = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr, filter));
            }
            
            return await GetList(expr, orderBy, page, limit);
        }
    }
}