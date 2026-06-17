using BackendAPI.Models;

namespace BackendAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task <int> GetTotalUsers();
        Task <List<User>> GetAllUsers();
        Task <bool> ChangeUserRole(int userId, string role);
    }
}