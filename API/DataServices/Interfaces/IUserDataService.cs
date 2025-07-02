using API.Entities;

namespace API.DataServices.Interfaces
{
    public interface IUserDataService
    {
        Task<User?> GetUserByKeyAsync(int key);
        Task<User?> GetUserByIdentificationAsync(string identification);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
