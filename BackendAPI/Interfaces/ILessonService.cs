using BackendAPI.Models;

namespace BackendAPI.Services
{
    public interface ILessonService
    {
        // READ ALL: Lấy danh sách bài học (Dùng Generic cho cả Admin và User)
        Task<IEnumerable<T>> GetAllLessonsAsync<T>() where T : class;

        // READ BY LEVEL: User chọn Level nào thì chỉ trả về các bài học của Level đó
        Task<IEnumerable<T>> GetLessonsByLevelAsync<T>(int levelId) where T : class;

        // READ BY ID: Xem chi tiết bài học
        Task<T?> GetLessonByIdAsync<T>(int id) where T : class;

        // CÁC HÀM CRUD DÀNH RIÊNG CHO ADMIN
        Task<Lesson> CreateLessonAsync(Lesson lessonData);
        Task<bool> UpdateLessonAsync(int id, Lesson lessonData);
        Task<bool> DeleteLessonAsync(int id);
    }
}