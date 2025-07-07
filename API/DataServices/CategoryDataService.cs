using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for category management, including CRUD operations.
    /// </summary>
    public class CategoryDataService(ApplicationDbContext applicationDbContext) : ICategoryDataService
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<Category?> GetCategoryByKeyAsync(int key)
        {
            return await _applicationDbContext.Categories.FindAsync(key);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _applicationDbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _applicationDbContext.Categories.ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _applicationDbContext.Categories.AddAsync(category);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// You are handling custom optimistic concurrency, not EF Core's automatic [Timestamp]-based mechanism.
        /// Therefore, EF Core will not detect concurrency conflicts on its own—you have to handle them manually.
        /// Here, RowVersion validation is done manually because an Oracle trigger increments it,
        /// and EF has no way of knowing that happened.
        /// </summary>
        public async Task UpdateCategoryAsync(Category category)
        {
            var originalCategory = await _applicationDbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Key == category.Key) ?? throw new InvalidOperationException($"Category with key {category.Key} not found.");

            // Manual concurrency validation 
            if (originalCategory.RowVersion != category.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the category was modified by another user.");

            // Check if any changes occurred
            bool isModified = originalCategory.Name != category.Name;

            // If no real changes were made
            if (!isModified)
                return;

            // EF Core will track this new object with the updated data 
            _applicationDbContext.Categories.Update(category);

            await _applicationDbContext.SaveChangesAsync(); // The trigger in the DB will increment ROWVERSION 
        }

        public async Task DeleteCategoryAsync(int key)
        {
            var category = await GetCategoryByKeyAsync(key);
            if (category != null)
            {
                _applicationDbContext.Categories.Remove(category);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
