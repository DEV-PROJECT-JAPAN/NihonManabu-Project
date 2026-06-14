using BackendAPI.Models;
using BackendAPI.Models.Data;
using BackendAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly JapaneseDbContext _context;

        public AdminService(JapaneseDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsers()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> ChangeUserRole(int userId, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return false;

            user.Role = role;
            await _context.SaveChangesAsync();  

            return true;
        }
    }
}