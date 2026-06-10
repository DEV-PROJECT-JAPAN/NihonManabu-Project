using FrontendRazorPage.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontendRazorPage.Core.Services
{
    public class VocabularyClientService : BaseClientService
    {
    
        private readonly string _apiBase = "api/vocabulary";

        public VocabularyClientService(HttpClient httpClient) : base(httpClient) { }


        public async Task<List<LessonModel>> GetLessonsAsync(int levelId) =>
            await _httpClient.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/lessons?levelId={levelId}") ?? new();

        public async Task<List<VocabularyModel>> GetCardsAsync(int lessonId) =>
            await _httpClient.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}") ?? new();

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
    }
}