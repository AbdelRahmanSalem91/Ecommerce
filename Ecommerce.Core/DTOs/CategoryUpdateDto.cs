
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.DTOs
{
    public class CategoryUpdateDto : CategoryDto
    {
        [Required]
        public int Id { get; set; }
    }
}
