using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for supplier management, including CRUD operations.
    /// </summary>
    public class SupplierDataService(ApplicationDbContext applicationDbContext) : ISupplierDataService
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<Supplier?> GetSupplierByKeyAsync(int key)
        {
            return await _applicationDbContext.Suppliers.FindAsync(key);
        }

        public async Task<Supplier?> GetSupplierByNameAsync(string name)
        {
            return await _applicationDbContext.Suppliers
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _applicationDbContext.Suppliers.ToListAsync();
        }

        public async Task AddSupplierAsync(Supplier category)
        {
            await _applicationDbContext.Suppliers.AddAsync(category);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// You are handling custom optimistic concurrency, not EF Core's automatic [Timestamp]-based mechanism.
        /// Therefore, EF Core will not detect concurrency conflicts on its own—you have to handle them manually.
        /// Here, RowVersion validation is done manually because an Oracle trigger increments it,
        /// and EF has no way of knowing that happened.
        /// </summary>
        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            var originalSupplier = await _applicationDbContext.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Key == supplier.Key) ?? throw new InvalidOperationException($"Supplier with key {supplier.Key} not found.");

            // Manual concurrency validation 
            if (originalSupplier.RowVersion != supplier.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the supplier was modified by another user.");

            // Check if any changes occurred
            bool isModified = originalSupplier.Name != supplier.Name;

            // If no real changes were made
            if (!isModified)
                return;

            // EF Core will track this new object with the updated data 
            _applicationDbContext.Suppliers.Update(supplier);

            await _applicationDbContext.SaveChangesAsync(); // The trigger in the DB will increment ROWVERSION 
        }

        public async Task DeleteSupplierAsync(int key)
        {
            var supplier = await GetSupplierByKeyAsync(key);
            if (supplier != null)
            {
                _applicationDbContext.Suppliers.Remove(supplier);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}