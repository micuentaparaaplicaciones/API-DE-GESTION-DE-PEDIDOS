using API.Dtos.ProductDtos;
using API.Entities;
using AutoMapper;

namespace API.MappingProfiles
{
    /// <summary>
    /// Maps between Product entity and Product DTOs for data transfer operations. 
    /// </summary>
    public class ProductProfile : Profile
    {
        public ProductProfile() {

            // Entity → ReadDto
            CreateMap<Product, ProductReadDto>();

            // CreateDto → Entity
            CreateMap <ProductCreateDto, Product>()
                .ForMember(dest => dest.Key, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore()) // Not modified when created
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore()) // Optional relationship
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore()); // Optional relationship 

            // UpdateDto → Entity
            CreateMap <ProductUpdateDto, Product>()
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore());
        }
    }
}
