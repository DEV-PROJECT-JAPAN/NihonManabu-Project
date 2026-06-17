using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJwtToken(User user);
    }
}
