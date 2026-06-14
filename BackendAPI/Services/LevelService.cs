using BackendAPI.DTOs;
using BackendAPI.Models;
using BackendAPI.Models.Data; // Đã giữ nguyên DbContext của bạn
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class LevelService : ILevelService
    {
        private readonly JapaneseDbContext _context;

        public LevelService(JapaneseDbContext context)
        {
            _context = context;
        }

        // ==================== 1. READ ALL (Dùng chung Generic) ====================
        public async Task<IEnumerable<T>> GetAllLevelsAsync<T>() where T : class
        {
            if (typeof(T) == typeof(Level)) // Luồng cho Admin
            {
                var adminData = await _context.Levels.AsNoTracking().ToListAsync();
                return (IEnumerable<T>)adminData;
            }

            if (typeof(T) == typeof(LevelDTO)) // Luồng cho User
            {
                var userData = await _context.Levels
                    .AsNoTracking()
                    .Select(l => new LevelDTO
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Description = l.Description
                    })
                    .ToListAsync();
                return (IEnumerable<T>)userData;
            }

            throw new ArgumentException("Kiểu dữ liệu không được hỗ trợ.");
        }

        // ==================== 2. READ BY ID (Dùng chung Generic) ====================
        public async Task<T?> GetLevelByIdAsync<T>(int id) where T : class
        {
            if (typeof(T) == typeof(Level)) // Luồng cho Admin
            {
                var level = await _context.Levels.FindAsync(id);
                return level as T;
            }

            if (typeof(T) == typeof(LevelDTO)) // Luồng cho User
            {
                var levelDto = await _context.Levels
                    .AsNoTracking()
                    .Where(l => l.Id == id)
                    .Select(l => new LevelDTO
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Description = l.Description
                    })
                    .FirstOrDefaultAsync();
                return levelDto as T;
            }

            throw new ArgumentException("Kiểu dữ liệu không được hỗ trợ.");
        }

        // ==================== 3. CREATE (CHỈ ADMIN) ====================
        public async Task<Level> CreateLevelAsync(Level levelData)
        {
            if (levelData == null) throw new ArgumentNullException(nameof(levelData));

            levelData.CreatedAt = DateTime.Now;
            levelData.UpdatedAt = DateTime.Now;

            _context.Levels.Add(levelData);
            await _context.SaveChangesAsync();

            return levelData;
        }

        // ==================== 4. UPDATE (CHỈ ADMIN) ====================
        public async Task<bool> UpdateLevelAsync(int id, Level levelData)
        {
            if (levelData == null) return false;

            var existingLevel = await _context.Levels.FindAsync(id);
            if (existingLevel == null) return false;

            // Cập nhật các thông tin sửa đổi
            existingLevel.Name = levelData.Name;
            existingLevel.Description = levelData.Description;
            existingLevel.UpdatedAt = DateTime.Now; // Ghi nhận thời gian cập nhật

            _context.Levels.Update(existingLevel);
            await _context.SaveChangesAsync();
            return true;
        }

        // ==================== 5. DELETE (CHỈ ADMIN) ====================
        public async Task<bool> DeleteLevelAsync(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level == null) return false;

            _context.Levels.Remove(level);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}