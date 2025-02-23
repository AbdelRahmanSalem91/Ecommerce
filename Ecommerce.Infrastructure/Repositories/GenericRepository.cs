using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Load
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region AddAsync
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        #endregion

        #region DeleteAsync
        public async Task DeleteAsync(int id)
        {
            T entity = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(entity);
        }
        #endregion

        #region GetAllAsync
        public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[]? includes)
        {
            if (includes is null)
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }

            IQueryable<T> query = _context.Set<T>().AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.AsNoTracking().ToListAsync();
        }
        #endregion

        #region GetByIdAsync
        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[]? includes)
        {
            if (includes is null)
            {
                return await _context.Set<T>().FindAsync(id);
            }

            IQueryable<T> query = _context.Set<T>().AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
        }
        #endregion

        #region UpdateAsync
        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        #endregion

        #region Count Async
        public async Task<int> CountAsync() => await _context.Set<T>().CountAsync();
        #endregion
    }
}
