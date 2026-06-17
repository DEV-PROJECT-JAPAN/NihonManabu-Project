using FrontendRazorPage.Models;
using FrontendRazorPage.Models.AdminModel;
using System.Net.Http;
using System.Net.Http.Json;



namespace FrontendRazorPage.Core.Services
{
    public class VocabularyClientService : BaseClientService
    {
    
        private readonly string _apiBase = "api/vocabulary";

        public VocabularyClientService(HttpClient httpClient) : base(httpClient) { }


        //public async Task<List<LessonModel>> GetLessonsAsync(int levelId) =>
        //    await _httpClient.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/lessons?levelId={levelId}") ?? new();

        public async Task<List<VocabularyModel>> GetCardsAsync(int lessonId) =>
            await _httpClient.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}") ?? new();


        public async Task<List<VocabularyAdminModel>> GetVocabulariesByLessonForAdminAsync(int lessonId)
        {
            // Gọi lên đầu API Admin của Backend để bốc nguyên bản thực thể có ngày tháng
            var response = await _httpClient.GetFromJsonAsync<List<VocabularyAdminModel>>($"{_apiBase}/admin/by-lesson/{lessonId}");
            return response ?? new List<VocabularyAdminModel>();
        }
        public async Task<bool> UpdateProgressAsync(UpdateLearningProgresByUserModel input)
        {
            try
            {
                // Bắn trực tiếp sang endpoint mới bóc tách ở BackendAPI
                var response = await _httpClient.PostAsJsonAsync($"{_apiBase}/update-LearningProgressByUser", input);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // Tránh sập ứng dụng Frontend khi Backend nghẽn mạch
            }
        }
        public async Task<bool> CreateVocabularyAsync(VocabularyAdminModel model)
        {
            try
            {
                // Bắn cục model phẳng chứa các trường Kanji, Hiragana, Romaji, TextToSpeak, ExampleSentence sang Backend
                var response = await _httpClient.PostAsJsonAsync($"{_apiBase}/admin", model);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 2. Hàm gọi API Xóa Từ vựng ra khỏi bài học (Dùng cho nút Xóa ở trang Index)
        /// </summary>
        public async Task<bool> DeleteVocabularyAsync(int id)
        {
            try
            {
                // Gọi trúng vào đầu API DELETE của Backend Admin (Ví dụ: DELETE api/vocabulary/admin/5)
                var response = await _httpClient.DeleteAsync($"{_apiBase}/admin/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 3. Hàm lấy chi tiết 1 từ vựng theo ID (Dùng để đổ dữ liệu cũ lên form trang Edit)
        /// </summary>
        public async Task<VocabularyAdminModel?> GetVocabularyByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<VocabularyAdminModel>($"{_apiBase}/admin/{id}");
                return response;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 4. Hàm gọi API Cập nhật Từ vựng (Dùng cho trang Edit)
        /// </summary>
        public async Task<bool> UpdateVocabularyAsync(int id, VocabularyAdminModel model)
        {
            try
            {
                // Gửi lệnh PUT kèm theo ID trên URL và Cục dữ liệu sửa đổi trong Body
                var response = await _httpClient.PutAsJsonAsync($"{_apiBase}/admin/{id}", model);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }



    }
}