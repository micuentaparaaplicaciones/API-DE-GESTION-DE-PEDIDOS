using API.Dtos.CategoryDtos;

namespace API.BusinessRules.Interfaces
{
    public interface ICategoryBusinessRules
    {
        Task<(bool Success, string? ErrorMessage)> ValidateNewCategoryAsync(CategoryCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> ValidateUpdatedCategoryAsync(CategoryUpdateDto dto);
    }
}
