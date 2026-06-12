using System.Net.Http.Json;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Core.Services
{
    public class PracticeClientService
    {
        private readonly HttpClient _http;
        private readonly string _apiBase = "https://localhost:7104/api/practice";

        public PracticeClientService(HttpClient http) => _http = http;

        public async Task<List<LevelModel>> GetLevelsAsync() =>
            await _http.GetFromJsonAsync<List<LevelModel>>($"{_apiBase}/levels") ?? new();

        public async Task<List<LessonModel>> GetLessonsAsync(int levelId) =>
            await _http.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/lessons?levelId={levelId}") ?? new();

        public async Task<List<VocabularyModel>> GetCardsAsync(int lessonId) =>
            await _http.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}") ?? new();
        //api practice
        //userVocabularyPractice
        public async Task<List<PracticeModel>> GetUserVocabularyPracticeAsync(int ListId) =>
            await _http.GetFromJsonAsync<List<PracticeModel>>($"{_apiBase}/practice?ListId={ListId}") ?? new();
        //userVocabularySystemPractice
        public async Task<List<PracticeModel>> GetUserVocabularySystemPracticeAsync(int LessonId) =>
            await _http.GetFromJsonAsync<List<PracticeModel>>($"{_apiBase}/practice?LessonId={LessonId}") ?? new();

        public async Task<List<PracticeModel>> GetUserFlashCardListAsync(int FolderId) =>
            await _http.GetFromJsonAsync<List<PracticeModel>>($"{_apiBase}/practice?FolderId={FolderId}") ?? new();
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