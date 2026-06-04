using FrontendRazorPage.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace FrontendRazorPage.Services
{
    public class GrammarClientService
    {

        private readonly HttpClient _http;
        private readonly string _apiBase = "https://localhost:7104/api/grammar";

        public GrammarClientService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<GrammarModel> GetGrammarByIdAsync(int grammarId)
        {
            if (grammarId <= 0) return new GrammarModel();
            try
            {
                // Bắn lệnh GET sang endpoint: /api/Grammar/{grammarId}
                // Nếu lỗi mạng hoặc null, toán tử ?? new() sẽ trả về object trống để chống sập giao diện
                return await _http.GetFromJsonAsync<GrammarModel>($"{_apiBase}/{grammarId}") ?? new();
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
                return await _http.GetFromJsonAsync<List<GrammarModel>>($"{_apiBase}/lesson/{lessonId}") ?? new();
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
                return await _http.GetFromJsonAsync<List<QuestionModel>>(url) ?? new();
            }
            catch
            {
                return new List<QuestionModel>();
            }
        }


    }
}