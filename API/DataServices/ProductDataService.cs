using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for product management, including CRUD operations.
    /// </summary>
    public class ProductDataService(ApplicationDbContext applicationDbContext) : IProductDataService
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<Product?> GetProductByKeyAsync(int key)
        {
            return await _applicationDbContext.Products.FindAsync(key);
        }

        public async Task<Product?> GetProductByNameAsync(string name)
        {
            return await _applicationDbContext.Products
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _applicationDbContext.Products.ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _applicationDbContext.Products.AddAsync(product);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var originalProduct = await _applicationDbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Key == product.Key)
                ?? throw new InvalidOperationException($"Product with key {product.Key} not found.");

            if (originalProduct.RowVersion != product.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the product was modified by another user.");

            bool isModified =
                originalProduct.Name != product.Name ||
                originalProduct.Detail != product.Detail ||
                originalProduct.Price != product.Price ||
                originalProduct.AvailableQuantity != product.AvailableQuantity ||
                originalProduct.Image.Length != product.Image.Length || // simple array size check
                originalProduct.SuppliedBy != product.SuppliedBy ||
                originalProduct.CategorizedBy != product.CategorizedBy;

            if (!isModified)
                return;

            _applicationDbContext.Products.Update(product);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int key)
        {
            var product = await GetProductByKeyAsync(key);
            if (product != null)
            {
                _applicationDbContext.Products.Remove(product);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
