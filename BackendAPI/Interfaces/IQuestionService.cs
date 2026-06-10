using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface IQuestionAdminService<T> where T : class
    {
        // 🎯 Hàm dùng chung cho cả User (DTO) và Admin (Model) thông qua kiểu T
        Task<IEnumerable<T>> GetQuestionsByLessonAsync(int lessonId);
        Task<T?> GetQuestionByIdAsync(int id);

        // 🔐 Các hàm ghi dữ liệu xuống DB luôn dùng Model gốc (Question) để quản lý cấu trúc
        Task<Question> CreateQuestionAsync(Question question);
        Task<bool> UpdateQuestionAsync(int id, Question question);
        Task<bool> DeleteQuestionAsync(int id);
    }
}