using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.SupplierDtos;

namespace API.BusinessRules
{
    /// <summary>
    /// Contains business rules for supplier management, such as validation for creating and updating suppliers.
    /// </summary>
    public class SupplierBusinessRules(ISupplierDataService supplierDataService) : ISupplierBusinessRules
    {
        private readonly ISupplierDataService _supplierDataService = supplierDataService;

        public async Task<(bool Success, string? ErrorMessage)> ValidateNewSupplierAsync(SupplierCreateDto supplierCreateDto)
        {
            var existingSupplier = await _supplierDataService.GetSupplierByNameAsync(supplierCreateDto.Name);
            if (existingSupplier != null)
                return (false, "Supplier name is already in use.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidateUpdatedSupplierAsync(SupplierUpdateDto supplierUpdateDto)
        {
            var existingSupplier = await _supplierDataService.GetSupplierByNameAsync(supplierUpdateDto.Name);
            if (existingSupplier != null && existingSupplier.Key != supplierUpdateDto.Key)
                return (false, "Supplier name is already in use by another supplier.");

            return (true, null);
        }
    }
}
