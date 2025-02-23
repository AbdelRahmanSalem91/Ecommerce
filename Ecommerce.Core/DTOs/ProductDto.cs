using Microsoft.AspNetCore.Http;

namespace Ecommerce.Core.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        //public virtual List<PhotoDto>? Photos { get; set; }
        public IFormFileCollection Photos { get; set; }
        public string? CategoryName { get; set; }
    }
}
