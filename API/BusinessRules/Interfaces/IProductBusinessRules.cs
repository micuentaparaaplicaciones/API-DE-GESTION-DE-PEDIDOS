using API.Dtos.ProductDtos;

namespace API.BusinessRules.Interfaces
{
    public interface IProductBusinessRules
    {
        Task<(bool Success, string? ErrorMessage)> ValidateNewProductAsync(ProductCreateDto productCreateDto);

        Task<(bool Success, string? ErrorMessage)> ValidateUpdatedProductAsync(ProductUpdateDto productUpdateDto);
    }
}

