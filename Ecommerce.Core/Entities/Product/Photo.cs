
namespace Ecommerce.Core.Entities.Product
{
    public class Photo : BaseEntity<int>
    {
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
}
