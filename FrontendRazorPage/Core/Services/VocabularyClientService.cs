using System.Net.Http.Json;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Core.Services
{
    public class VocabularyClientService
    {
        private readonly HttpClient _http;
        private readonly string _apiBase = "https://localhost:7104/api/vocabulary";

        public VocabularyClientService(HttpClient http) => _http = http;

        public async Task<List<LevelModel>> GetLevelsAsync() =>
            await _http.GetFromJsonAsync<List<LevelModel>>($"{_apiBase}/levels") ?? new();

        public async Task<List<LessonModel>> GetLessonsAsync(int levelId) =>
            await _http.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/lessons?levelId={levelId}") ?? new();

        public async Task<List<VocabularyModel>> GetCardsAsync(int lessonId) =>
            await _http.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}") ?? new();

        public async Task<bool> UpdateProgressAsync(UpdateLearningProgresByUserModel input)
        {
            try
            {
                // Bắn trực tiếp sang endpoint mới bóc tách ở BackendAPI
                var response = await _http.PostAsJsonAsync($"{_apiBase}/update-LearningProgressByUser", input);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // Tránh sập ứng dụng Frontend khi Backend nghẽn mạch
            }
        }
    }
}