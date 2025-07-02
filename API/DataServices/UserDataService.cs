using API.Data;
using API.DataServices.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataServices
{
    /// <summary>
    /// Provides data access methods for user management, including CRUD operations.
    /// </summary>
    public class UserDataService(UserDbContext userDbContext) : IUserDataService
    {
        private readonly UserDbContext _userDbContext = userDbContext;

        public async Task<User?> GetUserByKeyAsync(int key)
        {
            return await _userDbContext.Users.FindAsync(key);
        }

        public async Task<User?> GetUserByIdentificationAsync(string identification)
        {
            return await _userDbContext.Users
                .FirstOrDefaultAsync(u => u.Identification == identification);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userDbContext.Users.ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _userDbContext.Users.AddAsync(user);
            await _userDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Estás manejando concurrencia optimista personalizada, no la automática de EF Core con [Timestamp]. 
        /// Por eso, EF Core no detectará por sí solo los conflictos de concurrencia: tienes que hacerlo tú.
        /// Aquí la validación de RowVersion es manual, porque el trigger de Oracle lo incrementa, y EF no tiene cómo saber que eso pasó.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            var originalUser = await _userDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Key == user.Key) ?? throw new InvalidOperationException($"User with key {user.Key} not found.");

            // Validación manual de concurrencia
            if (originalUser.RowVersion != user.RowVersion)
                throw new DbUpdateConcurrencyException("RowVersion conflict: the user was modified by another user.");

            // Comparar si hubo algún cambio
            bool isModified = originalUser.Identification != user.Identification ||
                              originalUser.Name != user.Name ||
                              originalUser.Email != user.Email ||
                              originalUser.Phone != user.Phone ||
                              originalUser.Address != user.Address ||
                              originalUser.Role != user.Role ||
                              originalUser.ModifiedBy != user.ModifiedBy;

            // Si no hubo cambios reales
            if (!isModified)
                return; // Nada que hacer

            // EF Core rastreará este nuevo objeto con los datos actualizados
            _userDbContext.Users.Update(user);

            await _userDbContext.SaveChangesAsync(); // el trigger en la DB incrementará ROWVERSION
        }

        public async Task DeleteUserAsync(int key)
        {
            var user = await GetUserByKeyAsync(key);
            if (user != null)
            {
                _userDbContext.Users.Remove(user);
                await _userDbContext.SaveChangesAsync();
            }
        }
    }
}
