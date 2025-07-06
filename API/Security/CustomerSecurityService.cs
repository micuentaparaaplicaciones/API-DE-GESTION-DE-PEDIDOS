using API.Entities;
using API.Security.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Security
{
    /// <summary>
    /// Encapsulates password hashing and verification logic for customers.
    /// </summary>
    public class CustomerSecurityService(IPasswordHasher<Customer> passwordHasher) : ICustomerSecurityService
    {
        private readonly IPasswordHasher<Customer> _passwordHasher = passwordHasher;

        /// <summary>
        /// Hashes a plain text password for a given customer.
        /// </summary>
        public string HashPassword(Customer customer, string plainPassword)
        {
            return _passwordHasher.HashPassword(customer, plainPassword);
        }

        /// <summary>
        /// Verifies whether the provided password matches the customer's hashed password.
        /// </summary>
        public bool VerifyPassword(Customer customer, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(customer, customer.Password, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        /// <summary>
        /// Determines if the customer's stored password hash should be updated (e.g., because the password changed).
        /// </summary>
        public bool NeedsRehash(Customer customer, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(customer, customer.Password, providedPassword);
            return result != PasswordVerificationResult.Success;
        }
    }
}
