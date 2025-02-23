using AutoMapper;
using Ecommerce.Core.DTOs;
using Ecommerce.Core.Entities.Product;

namespace Ecommerce.API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(x => x.CategoryName,
                op => op.MapFrom(src => src.Category.Name))
                .ReverseMap();
        }
    }
}
