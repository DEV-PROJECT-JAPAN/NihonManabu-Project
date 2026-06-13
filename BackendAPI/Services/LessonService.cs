using BackendAPI.DTOs;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class LessonService : ILessonService
    {
        private readonly JapaneseDbContext _context;

        public LessonService(JapaneseDbContext context) => _context = context;

        // 1. READ ALL
        public async Task<IEnumerable<T>> GetAllLessonsAsync<T>() where T : class
        {
            if (typeof(T) == typeof(Lesson)) // Admin
            {
                var adminData = await _context.Lessons.AsNoTracking().ToListAsync();
                return adminData as IEnumerable<T> ?? throw new InvalidCastException();
                //Ép kiểu danh sách vừa lấy được về đúng chuẩn đầu ra của hàm.Nếu việc ép kiểu thất bại, hệ thống sẽ ném ra lỗ
            }
            if (typeof(T) == typeof(LessonDTO)) // User
            {
                var userData = await _context.Lessons.AsNoTracking()
                    .Select(l => new LessonDTO { 
                        Id = l.Id,
                        Title = l.Title,
                        Order = l.Order,
                        LevelId = l.LevelId })
                    .ToListAsync();
                return userData as IEnumerable<T> ?? throw new InvalidCastException();
            }
            throw new ArgumentException("Kiểu dữ liệu không hỗ trợ.");
        }

        // 2. READ BY LEVEL (Quan trọng cho trang hiển thị của User)
        public async Task<IEnumerable<T>> GetLessonsByLevelAsync<T>(int levelId) where T : class
        {
            var query = _context.Lessons.AsNoTracking().Where(l => l.LevelId == levelId);

            if (typeof(T) == typeof(Lesson))
            {
                return await query.ToListAsync() as IEnumerable<T> ?? throw new InvalidCastException();
            }
            if (typeof(T) == typeof(LessonDTO))
            {
                return await query.Select(l => new LessonDTO {
                    Id = l.Id, 
                    Title = l.Title, 
                    Order = l.Order,
                    LevelId = l.LevelId })
                                  .ToListAsync() as IEnumerable<T> ?? throw new InvalidCastException();
            }
            throw new ArgumentException("Kiểu dữ liệu không hỗ trợ.");
        }

        // 3. READ BY ID
        public async Task<T?> GetLessonByIdAsync<T>(int id) where T : class
        {
            if (typeof(T) == typeof(Lesson))
            {
                var lesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                return lesson as T;
            }
            if (typeof(T) == typeof(LessonDTO))
            {
                var lessonDto = await _context.Lessons.AsNoTracking()
                    .Where(l => l.Id == id)
                    .Select(l => new LessonDTO {
                        Id = l.Id,
                        Title = l.Title,
                        Order = l.Order,
                        LevelId = l.LevelId })
                    .FirstOrDefaultAsync();
                return lessonDto as T;
            }
            throw new ArgumentException("Kiểu dữ liệu không hỗ trợ.");
        }

        // 4. CREATE (ADMIN)
        public async Task<Lesson> CreateLessonAsync(Lesson lessonData)
        {
            if (lessonData == null) throw new ArgumentNullException(nameof(lessonData));
           
            _context.Lessons.Add(lessonData);
            await _context.SaveChangesAsync();
            return lessonData;
        }

        // 5. UPDATE (ADMIN)
        public async Task<bool> UpdateLessonAsync(int id, Lesson lessonData)
        {
            var existing = await _context.Lessons.FindAsync(id);
            if (existing == null || lessonData == null) return false;

            existing.Title = lessonData.Title;
            existing.Order = lessonData.Order;
            existing.LevelId = lessonData.LevelId; // Cho phép đổi bài học sang level khác
            existing.UpdatedAt = DateTime.Now;
            _context.Lessons.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        // 6. DELETE (ADMIN)
        public async Task<bool> DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return false;

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}