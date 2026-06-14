using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
