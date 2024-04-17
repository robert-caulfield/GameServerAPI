using System.Linq.Expressions;
using System.Reflection;

namespace GameServerAPI.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task<List<T>?> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1);
        Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, bool tracked = true, string? includeProperties = null);
        Task<T> CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
