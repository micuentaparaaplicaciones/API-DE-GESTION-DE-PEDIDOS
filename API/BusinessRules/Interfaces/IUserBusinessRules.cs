using API.Dtos.UserDtos;

namespace API.BusinessRules.Interfaces
{
    public interface IUserBusinessRules
    {
        Task<(bool Success, string? ErrorMessage)> ValidateNewUserAsync(UserCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> ValidateUpdatedUserAsync(UserUpdateDto dto);
    }
}
