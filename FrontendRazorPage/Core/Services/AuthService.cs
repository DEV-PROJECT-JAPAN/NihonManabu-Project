using FrontendRazorPage.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FrontendRazorPage.Core.Services
{
    public class AuthService : BaseApiServices
    {
        public AuthService(HttpClient http, IHttpContextAccessor httpContextAccessor) : base(http, httpContextAccessor)
        {
        }
        public async Task<string> SendOtpAsync(string email)
        {
            // Gửi dưới dạng Object để khớp với EmailRequest DTO ở Backend
            var data = new { Email = email };
            var response = await _httpClient.PostAsJsonAsync("api/auth/send-otp", data);

            if (response.IsSuccessStatusCode)
            {
                return "Đã gửi mã OTP thành công.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Lỗi: {error}";
            }
        }

        public async Task<AuthApiResponse?> VerifyAndRegisterAsync(RegisterViewModel data)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/auth/verify-and-register",
                    data);

            return await response.Content.ReadFromJsonAsync<AuthApiResponse>();
        }

        public async Task<AuthApiResponse> LoginAsync(LoginViewModel data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", data);

                // 1. Nếu thành công (200-299)
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthApiResponse>()
                           ?? new AuthApiResponse { Success = false, Message = "Dữ liệu trả về trống." };
                }

                // 2. Nếu lỗi (400, 401, 500...), kiểm tra Content-Type trước khi đọc JSON
                var content = await response.Content.ReadAsStringAsync();

                // Kiểm tra xem phản hồi có phải là JSON không bằng cách kiểm tra dấu { đầu tiên
                if (!string.IsNullOrWhiteSpace(content) && content.TrimStart().StartsWith("{"))
                {
                    try
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<AuthApiResponse>(content,
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                            ?? new AuthApiResponse { Success = false, Message = content };
                    }
                    catch
                    {
                        return new AuthApiResponse { Success = false, Message = content };
                    }
                }

                // Nếu nội dung không phải JSON (ví dụ lỗi HTML từ server), trả về nội dung thô
                return new AuthApiResponse { Success = false, Message = !string.IsNullOrEmpty(content) ? content : "Lỗi hệ thống không xác định." };
            }
            catch (Exception ex)
            {
                return new AuthApiResponse { Success = false, Message = $"Không thể kết nối máy chủ: {ex.Message}" };
            }
        }
        public async Task<ProfileViewModel?> GetProfileAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token)) return null;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/auth/profile");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthApiResponse>();
                return result?.Data;
            }
            return null;
        }

    }
}
