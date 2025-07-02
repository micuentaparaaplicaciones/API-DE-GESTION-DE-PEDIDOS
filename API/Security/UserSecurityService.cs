using API.Entities;
using API.Security.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Security
{
    /// <summary>
    /// Encapsulates password hashing and verification logic for users.
    /// </summary>
    public class UserSecurityService(IPasswordHasher<User> passwordHasher) : IUserSecurityService
    {
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        /// <summary>
        /// Hashes a plain text password for a given user.
        /// </summary>
        public string HashPassword(User user, string plainPassword)
        {
            return _passwordHasher.HashPassword(user, plainPassword);
        }

        /// <summary>
        /// Verifies whether the provided password matches the user's hashed password.
        /// </summary>
        public bool VerifyPassword(User user, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        /// <summary>
        /// Determines if the user's stored password hash should be updated (e.g., because the password changed).
        /// </summary>
        public bool NeedsRehash(User user, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, providedPassword);
            return result != PasswordVerificationResult.Success;
        }
    }
}
