using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.ProductDtos;

namespace API.BusinessRules
{
    /// <summary>
    /// Contains business rules for product management, such as validation for creating and updating products.
    /// </summary>
    public class ProductBusinessRules(IProductDataService productDataService) : IProductBusinessRules
    {
        private readonly IProductDataService _productDataService = productDataService;

        public async Task<(bool Success, string? ErrorMessage)> ValidateNewProductAsync(ProductCreateDto productCreateDto)
        {
            var existingProduct = await _productDataService.GetProductByNameAsync(productCreateDto.Name);
            if (existingProduct != null)
                return (false, "Product name is already in use.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidateUpdatedProductAsync(ProductUpdateDto productUpdateDto)
        {
            var existingProduct = await _productDataService.GetProductByNameAsync(productUpdateDto.Name);
            if (existingProduct != null && existingProduct.Key != productUpdateDto.Key)
                return (false, "Product name is already in use by another product.");

            return (true, null);
        }
    }
}
