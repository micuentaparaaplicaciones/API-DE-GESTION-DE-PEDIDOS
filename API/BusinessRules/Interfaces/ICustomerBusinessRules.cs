using API.Dtos.CustomerDtos;

namespace API.BusinessRules.Interfaces
{
    public interface ICustomerBusinessRules
    {
        Task<(bool Success, string? ErrorMessage)> ValidateNewCustomerAsync(CustomerCreateDto dto);
        //Task<(bool Success, string? ErrorMessage)> ValidateRegisterCustomerAsync(CustomerRegisterDto dto);
        Task<(bool Success, string? ErrorMessage)> ValidateUpdatedCustomerAsync(CustomerUpdateDto dto);
    }
}
