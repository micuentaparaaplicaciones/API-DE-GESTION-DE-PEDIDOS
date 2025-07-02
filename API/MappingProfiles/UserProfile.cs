using API.Dtos.UserDtos;
using API.Entities;
using AutoMapper;

namespace API.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity → ReadDto
            CreateMap<User, UserReadDto>();

            // CreateDto → Entity
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Key, opt => opt.Ignore()) // se genera automáticamente
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // se hashea manualmente
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore()) // se establece automáticamente
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore()) // se establece automáticamente
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore()) // relación opcional 
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore()) // relación opcional  
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()); // se genera automáticamente

            // UpdateDto → Entity
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));
        }
    }
}
