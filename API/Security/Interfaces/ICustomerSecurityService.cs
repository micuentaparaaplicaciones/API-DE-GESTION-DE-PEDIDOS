using API.Entities;

namespace API.Security.Interfaces
{
    public interface ICustomerSecurityService
    {
        string HashPassword(Customer customer, string plainPassword);
        bool VerifyPassword(Customer customer, string providedPassword);
        bool NeedsRehash(Customer customer, string providedPassword);
    }
}
