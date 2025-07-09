using API.Dtos.CategoryDtos;
using API.Entities;
using AutoMapper;

namespace API.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // Entity → ReadDto
            CreateMap<Category, CategoryReadDto>();

            // CreateDto → Entity
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.Key, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore()) // Optional relationship
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore()); // Optional relationship 


            // UpdateDto → Entity
            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore());
        }
    }
}
