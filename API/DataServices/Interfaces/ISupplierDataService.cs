using API.Entities;

namespace API.DataServices.Interfaces
{
    public interface ISupplierDataService
    {
        Task<Supplier?> GetSupplierByKeyAsync(int key);
        Task<Supplier?> GetSupplierByNameAsync(string name);
        Task<List<Supplier>> GetAllSuppliersAsync();
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int key);
    }
}
