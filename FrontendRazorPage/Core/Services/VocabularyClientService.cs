using System.Net.Http;
using System.Net.Http.Json;
using FrontendRazorPage.Models;
using System.Net.Http.Headers;
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

        // Hàm gửi dữ liệu Cấp độ mới lên Backend để tạo
        public async Task<bool> CreateLevelAsync(LevelModel model)
        {
            try
            {
                // Gọi lệnh POST chuyên dùng để Thêm mới
                var response = await _http.PostAsJsonAsync($"{_apiBase}/levels", model);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gọi API thêm mới Cấp độ: {ex.Message}");
                return false;
            }
        }

        // Hàm lấy chi tiết 1 Cấp độ theo ID (dùng để đổ dữ liệu cũ lên form)
        public async Task<LevelModel?> GetLevelByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<LevelModel>($"{_apiBase}/levels/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi] Lấy chi tiết Cấp độ: {ex.Message}");
                return null;
            }
        }

        // Hàm gửi dữ liệu đã sửa lên Backend (dùng PUT)
        public async Task<bool> UpdateLevelAsync(LevelModel model)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_apiBase}/levels/{model.Id}", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi] Cập nhật Cấp độ: {ex.Message}");
                return false;
            }
        }
        // Hàm gửi lệnh Xóa Cấp độ sang Backend
        public async Task<bool> DeleteLevelAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_apiBase}/levels/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi] Xóa Cấp độ: {ex.Message}");
                return false;
            }
        }
        #region ================= ADMIN LESSON CRUD =================

        public async Task<bool> CreateLessonAsync(LessonModel model)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_apiBase}/lessons", model);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<LessonModel?> GetLessonByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<LessonModel>($"{_apiBase}/lessons/{id}");
            }
            catch { return null; }
        }

        public async Task<bool> UpdateLessonAsync(LessonModel model)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_apiBase}/lessons/{model.Id}", model);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_apiBase}/lessons/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        #endregion

        #region ================= ADMIN VOCABULARY CRUD =================

        public async Task<List<LessonModel>> GetAllLessonsAdminAsync()
        {
            try { return await _http.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/all-lessons") ?? new(); }
            catch { return new(); }
        }

        //public async Task<bool> CreateVocabularyAsync(VocabularyModel model)
        //{
        //    try { return (await _http.PostAsJsonAsync($"{_apiBase}/vocabularies", model)).IsSuccessStatusCode; }
        //    catch { return false; }
        //}
        public async Task<bool> CreateVocabularyAsync(VocabularyModel model)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_apiBase}/vocabularies", model);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // BẪY LOG: Đọc chi tiết câu chửi của Backend và in ra cửa sổ Output
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n=================================================");
                    Console.WriteLine($"[TESTER BẮT BUG] BACKEND TỪ CHỐI LƯU TỪ VỰNG!");
                    Console.WriteLine($"Mã lỗi HTTP: {response.StatusCode}");
                    Console.WriteLine($"Chi tiết lỗi: {errorDetail}");
                    Console.WriteLine($"=================================================\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // BẪY LOG: Nếu rớt mạng hoặc sập Frontend
                Console.WriteLine($"\n[TESTER BẮT BUG] LỖI HỆ THỐNG/MẠNG: {ex.Message}\n");
                return false;
            }
        }
        public async Task<VocabularyModel?> GetVocabularyByIdAsync(int id)
        {
            try { return await _http.GetFromJsonAsync<VocabularyModel>($"{_apiBase}/vocabularies/{id}"); }
            catch { return null; }
        }

        public async Task<bool> UpdateVocabularyAsync(VocabularyModel model)
        {
            try { return (await _http.PutAsJsonAsync($"{_apiBase}/vocabularies/{model.Id}", model)).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<bool> DeleteVocabularyAsync(int id)
        {
            try { return (await _http.DeleteAsync($"{_apiBase}/vocabularies/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }

        #endregion


        public async Task<bool> ImportVocabulariesAsync(int lessonId, IFormFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Đọc luồng dữ liệu của file
                var fileStreamContent = new StreamContent(file.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                // "file" ở đây phải khớp với tên tham số IFormFile trong Controller Backend
                content.Add(fileStreamContent, "file", file.FileName);

                var response = await _http.PostAsync($"{_apiBase}/import/{lessonId}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

    }
}