using Ecommerce.Core.Entities.Product;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Sharing;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(ProductParams productParams)
        {
            var query = _context.Products
                .Include(x => x.Category)
                .Include(x => x.Photos)
                .AsNoTracking();

            if (productParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productParams.CategoryId);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                query = productParams.Sort switch
                {
                    "Asc" => query.OrderBy(p => p.Price),
                    "Desc" => query.OrderByDescending(p => p.Price),
                    _ => query.OrderBy(p => p.Price),
                };
            }

            query = query.Skip((productParams.PageSize) * (productParams.PageNumber - 1)).Take(productParams.PageSize);

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
