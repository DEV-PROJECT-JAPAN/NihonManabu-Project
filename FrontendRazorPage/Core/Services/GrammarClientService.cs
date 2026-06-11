using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using FrontendRazorPage.Models.AdminModel;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace FrontendRazorPage.Services
{
    public class GrammarClientService : BaseClientService
    {

       
        private readonly string _apiBase = "api/grammar";

        public GrammarClientService(HttpClient httpClient) : base(httpClient) { }
        

        public async Task<GrammarModel> GetGrammarByIdAsync(int grammarId)
        {
            if (grammarId <= 0) return new GrammarModel();
            try
            {
                // Bắn lệnh GET sang endpoint: /api/Grammar/{grammarId}
                // Nếu lỗi mạng hoặc null, toán tử ?? new() sẽ trả về object trống để chống sập giao diện
                return await _httpClient.GetFromJsonAsync<GrammarModel>($"{_apiBase}/{grammarId}") ?? new();
            }
            catch
            {
                // Nếu Backend sập hoặc mất mạng, trả về object trống để app chạy tiếp mượt mà
                return new GrammarModel();
            }
        }

        public async Task<List<GrammarModel>> GetGrammarByLessonAsync(int lessonId)
        {
            if (lessonId <= 0) return new List<GrammarModel>();

            try
            {
                // Bắn lệnh GET sang endpoint: /api/Grammar/lesson/{lessonId}
                // Nếu lỗi mạng hoặc null, toán tử ?? new() sẽ trả về danh sách trống để chống sập giao diện
                return await _httpClient.GetFromJsonAsync<List<GrammarModel>>($"{_apiBase}/lesson/{lessonId}") ?? new();
            }
            catch
            {
                // Nếu Backend sập hoặc mất mạng, trả về list trống để app chạy tiếp mượt mà
                return new List<GrammarModel>();
            }
        }

        public async Task<List<QuestionModel>> GetQuestionsByGrammarAsync(int grammarId, int questionType = 0)
        {
            if (grammarId <= 0) return new List<QuestionModel>();

            try
            {
                // Bắn URL dạng số: /api/Grammar/questions?grammarId=1&questionType=1
                string url = $"{_apiBase}/questions?grammarId={grammarId}&questionType={questionType}";
                return await _httpClient.GetFromJsonAsync<List<QuestionModel>>(url) ?? new();
            }
            catch
            {
                return new List<QuestionModel>();
            }
        }



        /// ////////////////////////////ADMIN//////////////////////////////



        // 1. Lấy danh sách cho Admin 
        public async Task<List<GrammarAdminModel>> GetAllForAdminAsync()
        {
            try
            {
                // Sửa thành đường dẫn tuyệt đối chuẩn xác kết hợp với _apiBase
                return await _httpClient.GetFromJsonAsync<List<GrammarAdminModel>>($"{_apiBase}/admin-list") ?? new();
            }
            catch
            {
                return new List<GrammarAdminModel>();
            }
        }

        // 2. Lấy chi tiết theo ID cho Admin
        public async Task<GrammarAdminModel?> GetByIdForAdminAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<GrammarAdminModel>($"{_apiBase}/admin/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAsync(GrammarAdminModel grammar)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBase}", grammar);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        // 4. Cập nhật mẫu ngữ pháp
        public async Task<bool> UpdateAsync(int id, GrammarAdminModel grammar)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBase}/{id}", grammar);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 5. Xóa mẫu ngữ pháp
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBase}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

}
