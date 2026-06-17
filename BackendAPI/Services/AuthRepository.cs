using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly JapaneseDbContext _context;

        // 2. Inject DbContext thông qua Constructor
        public AuthRepository(JapaneseDbContext context)
        {
            _context = context;
        }

        // 1. Kiểm tra email tồn tại chưa (Tạm thời luôn báo là chưa có để bạn dễ đăng ký)
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            // Đưa cả 2 vế về chữ thường để so sánh không phân biệt hoa thường
            bool exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());

            return exists;
        }

        // 2. Tạo User mới (Tạm thời trả về chính cái user đó để báo thành công)
        public async Task<User> CreateUserAsync(User user)
        {
            // Sau này chỗ này sẽ là: _context.Users.Add(user); await _context.SaveChangesAsync();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // 3. Lấy User theo Email (Tạm thời trả về null)
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            return user;
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
