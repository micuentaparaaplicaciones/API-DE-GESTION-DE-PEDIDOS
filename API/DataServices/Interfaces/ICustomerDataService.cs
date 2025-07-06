using API.Entities;

namespace API.DataServices.Interfaces
{
    public interface ICustomerDataService
    {
        Task<Customer?> GetCustomerByKeyAsync(int key);
        Task<Customer?> GetCustomerByIdentificationAsync(string identification);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<List<Customer>> GetAllCustomersAsync();
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
    }
}
