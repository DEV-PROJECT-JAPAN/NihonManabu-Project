using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AuthService _authService;
        public RegisterModel(AuthService authService) => _authService = authService;

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new RegisterViewModel();

        // SỬA: Thêm BindProperty để nó nhận giá trị từ form
        [BindProperty]
        public string OtpInput { get; set; } = string.Empty;

        [BindProperty]
        public int Step { get; set; } = 1;

        public string Message { get; set; } = string.Empty;

        public void OnGet() { Step = 1; }

        public async Task<IActionResult> OnPostSendOtpAsync()
        {
            // QUAN TRỌNG: Loại bỏ lỗi của OtpInput trước khi kiểm tra
            ModelState.Remove(nameof(OtpInput));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Message = "Lỗi: " + string.Join(" | ", errors);
                return Page();
            }

            var result = await _authService.SendOtpAsync(Input.Email);

            if (result.Contains("thành công", StringComparison.OrdinalIgnoreCase))
            {
                Message = result;
                Step = 2;
                ModelState.Clear(); // Xóa sạch để form Step 2 không bị dính lỗi cũ
            }
            else
            {
                Message = result;
                Step = 1;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            // Không cần remove ở đây vì chúng ta cần kiểm tra OtpInput tại bước này
            if (string.IsNullOrEmpty(OtpInput))
            {
                Message = "Vui lòng nhập mã OTP.";
                Step = 2;
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                Message = "Mật khẩu xác nhận không khớp.";
                Step = 2;
                return Page();
            }
            Input.OtpCode = OtpInput;
            var result = await _authService.VerifyAndRegisterAsync(Input);

            if (result != null && result.Success)
            {
                Message = result.Message;
                Step = 3;
            }
            else
            {
                Message = result?.Message ?? "Có lỗi xảy ra.";
                Step = 2;
            }
            return Page();
        }
    }
}
