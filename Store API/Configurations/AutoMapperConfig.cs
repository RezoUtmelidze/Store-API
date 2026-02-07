using AutoMapper;
using Store_API.Data;
using Store_API.Models;

namespace Store_API.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() 
        {
            CreateMap<Store, StoreDTO>().ReverseMap();
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryIDs, opt => opt.MapFrom(src => src.Categories.Select(c => c.ID)));
            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
