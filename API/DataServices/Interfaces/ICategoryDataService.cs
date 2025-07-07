using API.Entities;

namespace API.DataServices.Interfaces
{
    public interface ICategoryDataService
    {
        Task<Category?> GetCategoryByKeyAsync(int key);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<List<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int key);
    }
}
