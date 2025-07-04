using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.UserDtos;

namespace API.BusinessRules
{
    /// <summary>
    /// Contains business rules for user management, such as validation for creating and updating users.
    /// </summary>
    public class UserBusinessRules(IUserDataService userDataService) : IUserBusinessRules
    {
        private readonly IUserDataService _userDataService = userDataService;

        public async Task<(bool Success, string? ErrorMessage)> ValidateNewUserAsync(UserCreateDto userCreateDto)
        {
            var existingIdentificationUser = await _userDataService.GetUserByIdentificationAsync(userCreateDto.Identification);
            if (existingIdentificationUser != null)
                return (false, "Identification is already in use.");

            var existingEmailUser = await _userDataService.GetUserByEmailAsync(userCreateDto.Email);
            if (existingEmailUser != null)
                return (false, "Email is already in use.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidateUpdatedUserAsync(UserUpdateDto userCreateDto)
        {
            var existingIdentificationUser = await _userDataService.GetUserByIdentificationAsync(userCreateDto.Identification);
            if (existingIdentificationUser != null && existingIdentificationUser.Key != userCreateDto.Key)
                return (false, "Identification is already in use by another user.");

            var existingEmailUser = await _userDataService.GetUserByEmailAsync(userCreateDto.Email);
            if (existingEmailUser != null && existingEmailUser.Key != userCreateDto.Key)
                return (false, "Email is already in use by another user.");

            return (true, null);
        }
    }
}
