using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.CustomerDtos;

namespace API.BusinessRules
{
    /// <summary>
    /// Contains business rules for customer management, such as validation for creating and updating users.
    /// </summary>
    public class CustomerBusinessRules(ICustomerDataService customerDataService) : ICustomerBusinessRules
    {
        private readonly ICustomerDataService _userDataService = customerDataService;

        public async Task<(bool Success, string? ErrorMessage)> ValidateNewCustomerAsync(CustomerCreateDto customerCreateDto)
        {
            var existingIdentificationCustomer = await _userDataService.GetCustomerByIdentificationAsync(customerCreateDto.Identification);
            if (existingIdentificationCustomer != null)
                return (false, "Identification is already in use.");

            var existingEmailCustomer = await _userDataService.GetCustomerByEmailAsync(customerCreateDto.Email);
            if (existingEmailCustomer != null)
                return (false, "Email is already in use.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidateUpdatedCustomerAsync(CustomerUpdateDto customerUpdateDto)
        {
            var existingIdentificationCustomer = await _userDataService.GetCustomerByIdentificationAsync(customerUpdateDto.Identification);
            if (existingIdentificationCustomer != null && existingIdentificationCustomer.Key != customerUpdateDto.Key)
                return (false, "Identification is already in use by another customer.");

            var existingEmailCustomer = await _userDataService.GetCustomerByEmailAsync(customerUpdateDto.Email);
            if (existingEmailCustomer != null && existingEmailCustomer.Key != customerUpdateDto.Key)
                return (false, "Email is already in use by another customer.");

            return (true, null);
        }
    }
}