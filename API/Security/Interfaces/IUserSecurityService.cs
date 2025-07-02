using API.Entities;

namespace API.Security.Interfaces
{
    public interface IUserSecurityService
    {
        string HashPassword(User user, string plainPassword);
        bool VerifyPassword(User user, string providedPassword);
        bool NeedsRehash(User user, string providedPassword);
    }
}
