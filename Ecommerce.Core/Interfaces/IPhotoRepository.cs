
using Ecommerce.Core.Entities.Product;

namespace Ecommerce.Core.Interfaces
{
    public interface IPhotoRepository : IGenericRepository<Photo>
    {
        Task<IEnumerable<Photo>> GetPhotoByProductId(int id);
    }
}
