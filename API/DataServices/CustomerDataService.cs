using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for customer management, including CRUD operations.
    /// </summary>
    public class CustomerDataService(ApplicationDbContext applicationDbContext) : ICustomerDataService
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<Customer?> GetCustomerByKeyAsync(int key)
        {
            return await _applicationDbContext.Customers.FindAsync(key);
        }

        public async Task<Customer?> GetCustomerByIdentificationAsync(string identification)
        {
            return await _applicationDbContext.Customers
                .FirstOrDefaultAsync(u => u.Identification == identification);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _applicationDbContext.Customers
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _applicationDbContext.Customers.ToListAsync();
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _applicationDbContext.Customers.AddAsync(customer);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// You are handling custom optimistic concurrency, not EF Core's automatic [Timestamp]-based mechanism.
        /// Therefore, EF Core will not detect concurrency conflicts on its own—you have to handle them manually.
        /// Here, RowVersion validation is done manually because an Oracle trigger increments it,
        /// and EF has no way of knowing that happened.
        /// </summary>
        public async Task UpdateCustomerAsync(Customer customer)
        {
            var originalCustomer = await _applicationDbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Key == customer.Key) ?? throw new InvalidOperationException($"Customer with key {customer.Key} not found.");

            // Manual concurrency validation 
            if (originalCustomer.RowVersion != customer.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the customer was modified by another user.");

            // Check if any changes occurred 
            bool isModified = originalCustomer.Identification != customer.Identification ||
                              originalCustomer.Name != customer.Name ||
                              originalCustomer.Email != customer.Email ||
                              originalCustomer.Phone != customer.Phone ||
                              originalCustomer.Address != customer.Address ||
                              originalCustomer.ModifiedBy != customer.ModifiedBy;

            // If no real changes were made 
            if (!isModified)
                return;

            // EF Core will track this new object with the updated data 
            _applicationDbContext.Customers.Update(customer);

            await _applicationDbContext.SaveChangesAsync(); // The trigger in the DB will increment ROWVERSION 
        }

        public async Task DeleteCustomerAsync(int key)
        {
            var customer = await GetCustomerByKeyAsync(key);
            if (customer != null)
            {
                _applicationDbContext.Customers.Remove(customer);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
