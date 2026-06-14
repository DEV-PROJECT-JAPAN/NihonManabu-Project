using FrontendRazorPage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Payment
{
    public class IndexModel : PageModel
    {
        private readonly PaymentClientService _paymentService;
        public string CurrentRole { get; set; } = "User";

        public IndexModel(PaymentClientService paymentService)
        {
            _paymentService = paymentService;
        }

        public void OnGet() { }

        public async Task<JsonResult> OnGetGenerateQrAsync()
        {
            // 1. Hứng Token từ Javascript truyền lên
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new JsonResult(new { success = false, message = "LỖI FE: Không nhận được Token từ trình duyệt." });
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            // 2. Chuyển cho Service gọi Backend
            string qrUrl = await _paymentService.GenerateVipQrAsync(token);

            if (!string.IsNullOrEmpty(qrUrl))
            {
                return new JsonResult(new { success = true, qrUrl = qrUrl });
            }

            return new JsonResult(new { success = false, message = "LỖI BE: Máy chủ từ chối cấp mã QR (Có thể do Token sai hoặc lỗi Database)." });
        }
    }
}