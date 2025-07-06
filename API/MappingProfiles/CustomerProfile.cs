using API.Dtos.CustomerDtos;
using API.Dtos.UserDtos;
using API.Entities;
using AutoMapper;

namespace API.MappingProfiles
{
    /// <summary>
    /// Maps between Customer entity and Customer DTOs for data transfer operations. 
    /// </summary>
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Entity → ReadDto
            CreateMap<Customer, CustomerReadDto>();

            // CreateDto → Entity
            CreateMap<CustomerCreateDto, Customer>()
                .ForMember(dest => dest.Key, opt => opt.Ignore()) // Automatically generated
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Manually hashed
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore()) // Automatically set
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore()) // Optional relationship
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore()) // Optional relationship 
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()); // Automatically generated

            // RegisterDto → Entity
            CreateMap<CustomerRegisterDto, Customer>()
                .ForMember(dest => dest.Key, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()) 
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModificationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

            // UpdateDto → Entity
            CreateMap<CustomerUpdateDto, Customer>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));
        }
    }
}
