
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.DTOs
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
