using API.Dtos.SupplierDtos;

namespace API.BusinessRules.Interfaces
{
    public interface ISupplierBusinessRules
    {
        Task<(bool Success, string? ErrorMessage)> ValidateNewSupplierAsync(SupplierCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> ValidateUpdatedSupplierAsync(SupplierUpdateDto dto);
    }
}
