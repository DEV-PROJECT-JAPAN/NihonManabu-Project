namespace FrontendRazorPage.Models
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } // Thêm để so khớp mật khẩu
        public string? OtpCode { get; set; } // Thêm để nhận mã OTP từ form
    }

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthApiResponse
    {
        public string Message { get; set; }
        public string? Email { get; set; }
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
        public ProfileViewModel? Data { get; set; }
    }

    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TotalExp { get; set; }
        public int CurrentStreak { get; set; } // Số ngày Streak
    }
}
