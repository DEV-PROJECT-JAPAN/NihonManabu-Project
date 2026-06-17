using System.Net.Http.Json;
using BackendAPI.DTOs;
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
        //lấy danh sách lesson của user đã học
        public async Task<List<LessonModel>> GetUserLessonsAsync() =>
            await _http.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/user-lessons") ?? new();
        //userVocabularyPractice
        public async Task<List<PracticeModel>> GetUserVocabularyPracticeAsync(int ListId) =>
            await _http.GetFromJsonAsync<List<PracticeModel>>($"{_apiBase}/practice?ListId={ListId}") ?? new();
        //userVocabularySystemPractice
        public async Task<List<VocabularyModel>> GetVocabularySystemPracticeAsync() =>
            await _http.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/practice-system") ?? new();

        public async Task<List<PracticeModel>> GetFolderVocabAsync(int FolderId) =>
            await _http.GetFromJsonAsync<List<PracticeModel>>($"{_apiBase}/practice-user?FolderId={FolderId}") ?? new();
        
        public async Task<List<UserFlashcardList>> GetUserFoldersAsync() =>
            await _http.GetFromJsonAsync<List<UserFlashcardList>>($"{_apiBase}/user-folders") ?? new();
        //đọc file excel của user để lấy danh sách từ vựng cho folder practice
        public async Task<bool> UploadExcelToApiAsync(string folderName, string description, IFormFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // 1. Đóng gói cái Tên thư mục
                content.Add(new StringContent(folderName), "folderName");

                // 2. Đóng gói cái Description
                content.Add(new StringContent(description), "description");

                // 3. Đóng gói cái File
                using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                // Chữ "file" ở đây phải khớp đúng tên biến IFormFile file ở Controller Backend
                content.Add(fileContent, "file", file.FileName);

                // Bắn sang Backend API
                var response = await _http.PostAsync($"{_apiBase}/upload-folder-excel", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi file đi: {ex.Message}");
                return false;
            }
        }
        //xóa folder practice của user
        public async Task<bool> DeleteFolderAsync(int folderId)
        {
            try
            {
                // Gọi API với phương thức DELETE
                var response = await _http.DeleteAsync($"{_apiBase}/delete-folder/{folderId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

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