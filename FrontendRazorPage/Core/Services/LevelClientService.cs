using FrontendRazorPage.Models;
using FrontendRazorPage.Models.AdminModel;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace FrontendRazorPage.Core.Services
{
    public class LevelClientService : BaseClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "/api/levels"; // Đổi lại port Backend của bạn

        public LevelClientService(HttpClient httpClient) : base(httpClient) => _httpClient = httpClient;


        public async Task<List<LevelModel>> GetLevelsAsync() =>
           await _httpClient.GetFromJsonAsync<List<LevelModel>>($"{_baseUrl}") ?? new();


        // 1. Lấy danh sách cho ADMIN (Trả về LevelAdminModel)
        public async Task<List<LevelAdminModel>> GetLevelsForAdminAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<LevelAdminModel>>($"{_baseUrl}/admin") ?? new();
        }


        // 2. Lấy chi tiết cho ADMIN
        public async Task<LevelAdminModel?> GetLevelByIdForAdminAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<LevelAdminModel>($"{_baseUrl}/admin/{id}");
        }

        // 3. ADMIN tạo mới (Truyền body dữ liệu thô)
        public async Task<bool> CreateLevelAsync(LevelAdminModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/admin", model);
            return response.IsSuccessStatusCode;
        }

        // 4. ADMIN cập nhật
        public async Task<bool> UpdateLevelAsync(int id, LevelAdminModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/admin/{id}", model);
            return response.IsSuccessStatusCode;
        }

        // 5. ADMIN xóa
        public async Task<bool> DeleteLevelAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/admin/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}