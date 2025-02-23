using Ecommerce.Core.Entities.Product;
using Ecommerce.Core.Sharing;

namespace Ecommerce.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetAllAsync(ProductParams productParams);
    }
}
