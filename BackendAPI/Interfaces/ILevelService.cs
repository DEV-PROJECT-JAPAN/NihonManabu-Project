using BackendAPI.DTOs;
using BackendAPI.Models;

namespace BackendAPI.Services
{
    public interface ILevelService
    {
        // READ ALL: Lấy danh sách
        Task<IEnumerable<T>> GetAllLevelsAsync<T>() where T : class;

        // READ BY ID: Lấy chi tiết theo Id
        Task<T?> GetLevelByIdAsync<T>(int id) where T : class;

        // CREATE: Admin tạo mới bằng Model, User (nếu có quyền) tạo bằng DTO
        Task<Level> CreateLevelAsync(Level entityDto);

        // UPDATE: Cập nhật dữ liệu
        Task<bool> UpdateLevelAsync(int id, Level entityDto);

        // DELETE: Xóa theo Id (Hàm này dùng chung hoàn toàn vì chỉ cần Id)
        Task<bool> DeleteLevelAsync(int id);
    }
}