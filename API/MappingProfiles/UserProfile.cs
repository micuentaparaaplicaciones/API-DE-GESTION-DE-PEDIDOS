using API.Dtos.UserDtos;
using API.Entities;
using AutoMapper;

namespace API.MappingProfiles
{
    /// <summary>
    /// Maps between User entity and User DTOs for data transfer operations. 
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity → ReadDto
            CreateMap<User, UserReadDto>();

            // CreateDto → Entity
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Key, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Manually hashed
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore()) // Optional relationship
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore()) // Optional relationship 
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()); // Automatically generated

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
