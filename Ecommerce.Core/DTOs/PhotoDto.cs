using Microsoft.AspNetCore.Http;

namespace Ecommerce.Core.DTOs
{
    public class PhotoDto
    {
        public IFormFile ProductImage { get; set; }
        public string ImageName { get; set; }
        public string Imageurl { get; set; }
        public int ProductId { get; set; }
    }
}
