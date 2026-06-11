using FrontendRazorPage.Models; // Hãy đảm bảo bạn có LessonModel hoặc LessonAdminModel ở đây
using System.Net.Http.Json;

using FrontendRazorPage.Models.AdminModel;
using System.Net.Http;

namespace FrontendRazorPage.Core.Services
{
    public class LessonClientService : BaseClientService
    {
        private readonly string _baseUrl = "api/lessons";

        public LessonClientService(HttpClient httpClient) : base(httpClient) { }

        // 1. Gọi API lấy bài học cho User dựa theo LevelId


        public async Task<List<LessonModel>> GetLessonsByLevelAsync(int levelId)
        {
           
                return await _httpClient.GetFromJsonAsync<List<LessonModel>>($"{_baseUrl}/level/{levelId}") ?? new();
          
        }
        // Thêm hàm này vào Frontend Client Service cho User gọi
        public async Task<LessonModel?> GetLessonByIdAsync(int id)
        {
            try
            {
                // Gọi chung vào đầu API admin hoặc một đầu API công khai đều được
                return await _httpClient.GetFromJsonAsync<LessonModel>($"api/lessons/admin/{id}");
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<LessonAdminModel>> GetLessonsByLevelForAdminAsync(int levelId)
        {
            try
            {
                // 🟢 Gọi vào đầu API admin (nhánh trả về thực thể gốc có ngày tháng)
                var response = await _httpClient.GetFromJsonAsync<List<LessonAdminModel>>($"{_baseUrl}/admin/by-level/{levelId}");
                return response ?? new List<LessonAdminModel>();
            }
            catch
            {
                return new List<LessonAdminModel>();
            }
        }
        // 2. Lấy toàn bộ danh sách bài học cho ADMIN
        public async Task<List<LessonAdminModel>> GetLessonsForAdminAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<LessonAdminModel>>($"{_baseUrl}/admin") ?? new();
        }
         
            
// 3. Lấy chi tiết bài học cho ADMIN
public async Task<LessonAdminModel?> GetLessonByIdForAdminAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<LessonAdminModel>($"{_baseUrl}/admin/{id}");
            }
            catch
            {
                return null;
            }
        }

        // 4. ADMIN tạo mới
        public async Task<bool> CreateLessonAsync(LessonAdminModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/admin", model);
            return response.IsSuccessStatusCode;
        }

        // 5. ADMIN cập nhật
        public async Task<bool> UpdateLessonAsync(int id, LessonAdminModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/admin/{id}", model);
            return response.IsSuccessStatusCode;
        }

        // 6. ADMIN xóa
        public async Task<bool> DeleteLessonAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/admin/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}