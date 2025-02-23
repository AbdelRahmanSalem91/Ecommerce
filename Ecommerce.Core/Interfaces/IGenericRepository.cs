using System.Linq.Expressions;

namespace Ecommerce.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[]? includes);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[]? includes);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
