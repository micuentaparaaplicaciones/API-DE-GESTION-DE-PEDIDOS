using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.CategoryDtos;

namespace API.BusinessRules
{
    /// <summary>
    /// Contains business rules for category management, such as validation for creating and updating categories.
    /// </summary>
    public class CategoryBusinessRules(ICategoryDataService categoryDataService) : ICategoryBusinessRules
    {
        private readonly ICategoryDataService _categoryDataService = categoryDataService;

        public async Task<(bool Success, string? ErrorMessage)> ValidateNewCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            var existingCategory = await _categoryDataService.GetCategoryByNameAsync(categoryCreateDto.Name);
            if (existingCategory != null)
                return (false, "Category name is already in use.");
            
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidateUpdatedCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            var existingCategory = await _categoryDataService.GetCategoryByNameAsync(categoryUpdateDto.Name);
            if (existingCategory != null && existingCategory.Key != categoryUpdateDto.Key)
                return (false, "Category name is already in use by another category.");
            
            return (true, null);
        }
    }
}
