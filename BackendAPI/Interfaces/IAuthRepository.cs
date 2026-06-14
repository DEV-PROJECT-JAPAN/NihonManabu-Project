using BackendAPI.Interfaces;
using BackendAPI.Models;
namespace BackendAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
    }
}
