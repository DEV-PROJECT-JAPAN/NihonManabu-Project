using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models; // Chứa dữ liệu giao diện hoặc DTO thu gọn của bạn
using FrontendRazorPage.Models.AdminModel; // Chứa bảng gốc Models

namespace FrontendRazorPage.Services
{
    public class QuestionClientService : BaseClientService
    {
       

        // 🌟 SỬA ĐỔI 1: Trỏ chính xác về đúng Endpoint Quản trị câu hỏi tại Backend
        private readonly string _apiBaseUrl = "api/QuestionAdmin";

        public QuestionClientService(HttpClient httpClient) : base(httpClient) { }  
        

        // 🔐 1. [ADMIN] Gọi API lấy danh sách câu hỏi theo bài học dạng đầy đủ Model gốc
        public async Task<List<QuestionAdminModel>> GetQuestionsByLessonForAdminAsync(int lessonId)
        {
            if (lessonId <= 0) return new List<QuestionAdminModel>();
            try
            {
                // Gọi đầu api/QuestionAdmin/admin/lesson/{lessonId} công khai hôm trước
                var response = await _httpClient.GetFromJsonAsync<List<QuestionAdminModel>>($"{_apiBaseUrl}/admin/lesson/{lessonId}");
                return response ?? new List<QuestionAdminModel>();
            }
            catch (HttpRequestException)
            {
                return new List<QuestionAdminModel>();
            }
        }

        // 🌐 2. [USER] Gọi API lấy danh sách câu hỏi theo bài học dạng rút gọn DTO
        // 🌟 SỬA ĐỔI 2: Đổi kiểu trả về thành List<QuestionDTO> (hoặc Model giao diện thích hợp của học viên)
        public async Task<List<QuestionModel>> GetQuestionsByLessonForUserAsync(int lessonId)
        {
            if (lessonId <= 0) return new List<QuestionModel>();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<QuestionModel>>($"{_apiBaseUrl}/user/lesson/{lessonId}");
                return response ?? new List<QuestionModel>();
            }
            catch (HttpRequestException)
            {
                return new List<QuestionModel>();
            }
        }

        // 🔍 3. [CHUNG] Gọi API lấy chi tiết một câu hỏi theo ID hệ thống
        public async Task<QuestionAdminModel?> GetQuestionByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<QuestionAdminModel>($"{_apiBaseUrl}/{id}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        // ➕ 4. [ADMIN] Gửi Request thêm mới toàn bộ cụm dữ liệu câu hỏi lồng kèm đáp án
        public async Task<bool> CreateQuestionAsync(QuestionAdminModel question)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, question);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        // ✏️ 5. [ADMIN] Gửi Request cập nhật sửa đổi, đồng bộ mảng câu trả lời Answers
        public async Task<bool> UpdateQuestionAsync(int id, QuestionAdminModel question)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", question);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        // ❌ 6. [ADMIN] Gửi lệnh xóa câu hỏi vĩnh viễn khỏi hệ thống database
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}