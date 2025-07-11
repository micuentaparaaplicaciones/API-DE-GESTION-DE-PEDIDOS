using API.Entities;

namespace API.DataServices.Interfaces
{
    public interface IProductDataService
    {
        Task<Product?> GetProductByKeyAsync(int key);

        Task<Product?> GetProductByNameAsync(string name);

        Task<List<Product>> GetAllProductsAsync();

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(int key);
    }
}
