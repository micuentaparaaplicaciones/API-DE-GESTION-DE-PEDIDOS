using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for user management, including CRUD operations.
    /// </summary>
    public class UserDataService(ApplicationDbContext applicationDbContext) : IUserDataService
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<User?> GetUserByKeyAsync(int key)
        {
            return await _applicationDbContext.Users.FindAsync(key);
        }

        public async Task<User?> GetUserByIdentificationAsync(string identification)
        {
            return await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.Identification == identification);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _applicationDbContext.Users.ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _applicationDbContext.Users.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// You are handling custom optimistic concurrency, not EF Core's automatic [Timestamp]-based mechanism.
        /// Therefore, EF Core will not detect concurrency conflicts on its own—you have to handle them manually.
        /// Here, RowVersion validation is done manually because an Oracle trigger increments it,
        /// and EF has no way of knowing that happened.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            var originalUser = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Key == user.Key) ?? throw new InvalidOperationException($"User with key {user.Key} not found.");

            // Manual concurrency validation 
            if (originalUser.RowVersion != user.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the user was modified by another user.");

            // Check if any changes occurred 
            bool isModified = originalUser.Identification != user.Identification ||
                              originalUser.Name != user.Name ||
                              originalUser.Email != user.Email ||
                              originalUser.Phone != user.Phone ||
                              originalUser.Address != user.Address ||
                              originalUser.Role != user.Role ||
                              originalUser.ModifiedBy != user.ModifiedBy;

            // If no real changes were made 
            if (!isModified)
                return;

            // EF Core will track this new object with the updated data 
            _applicationDbContext.Users.Update(user);

            await _applicationDbContext.SaveChangesAsync(); // The trigger in the DB will increment ROWVERSION 
        }

        public async Task DeleteUserAsync(int key)
        {
            var user = await GetUserByKeyAsync(key);
            if (user != null)
            {
                _applicationDbContext.Users.Remove(user);
                await _applicationDbContext.SaveChangesAsync();
            } 
        }
    }
}
