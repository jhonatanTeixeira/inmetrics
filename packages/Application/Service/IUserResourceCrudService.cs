using System.Linq.Expressions;
using Domain.Document;

namespace Application.Service
{
    public interface IUserResourceCrudService<T> : ICrudService<T> where T : IUserResource
    {
        public Task<List<T>> GetListForUser(string userId, Expression<Func<T, bool>>? filter = null, string orderBy = "Id", int page = 1, int limit = 30);
    }
}