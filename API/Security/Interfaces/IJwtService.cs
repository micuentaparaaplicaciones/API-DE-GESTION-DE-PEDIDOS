using API.Entities;

namespace API.Security.Interfaces
{
    public interface IJwtService
    {
        string GenerateUserToken(User user);

        string GenerateCustomerToken(Customer customer);
    }
}
