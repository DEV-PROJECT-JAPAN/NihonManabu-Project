using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🛡️ Bật khiên bảo vệ JWT
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        private readonly IPaymentWebhookService _webhookService;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentService paymentService, IPaymentWebhookService webhookService, IConfiguration config)
        {
            _paymentService = paymentService;
            _webhookService = webhookService;
            _config = config;
        }

        [HttpPost("generate-vip-qr")]
        public async Task<IActionResult> GenerateQrCode()
        {
            try
            {
                // Bóc tách Id từ Token JWT (Tìm Claim NameIdentifier hoặc Id)
                var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "Id");
                if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ hoặc thiếu ID người dùng." });
                }

                string qrImageUrl = await _paymentService.GenerateVipPaymentQrAsync(userId);
                return Ok(new { success = true, qrUrl = qrImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi Backend: " + ex.Message });
            }
        }
        // CỔNG NHẬN WEBHOOK TỪ NGÂN HÀNG
        [HttpPost("bank-webhook")]
        [AllowAnonymous] // 🛡️ Cho phép bên ngoài gọi vào không cần JWT Token
        public async Task<IActionResult> ReceiveBankWebhook([FromBody] BankWebhookDTO webhookData)
        {
            try
            {
                // 1. CHỐNG HACKER: Kiểm tra mã bảo mật API Key
                // Mã này lấy từ file appsettings.json (VD: ALPHA_PGR_SECRET_KEY_9999)
                var mySecretKey = _config["VietQRConfig:WebhookSecret"];
                var incomingApiKey = Request.Headers["x-api-key"].ToString();

                // Nếu không gửi mã hoặc mã sai -> Đuổi cổ ngay lập tức
                if (string.IsNullOrEmpty(mySecretKey) || incomingApiKey != mySecretKey)
                {
                    return Unauthorized(new { success = false, message = "Từ chối truy cập: Sai mã bảo mật Webhook!" });
                }

                // 2. Giao cho Service xử lý Database
                bool isSuccess = await _webhookService.ProcessWebhookAsync(webhookData);

                if (isSuccess)
                {
                    // Bắt buộc trả về HTTP 200 OK để Bot ngân hàng biết hệ thống đã xử lý xong
                    return Ok(new { success = true, message = "Ghi nhận giao dịch thành công" });
                }

                return BadRequest(new { success = false, message = "Không thể xử lý giao dịch này" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("check-status")]
        [Authorize] // 🛡️ Bắt buộc có JWT
        public async Task<IActionResult> CheckVipStatus()
        {
            try
            {
                var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "Id");
                if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
                {
                    return Unauthorized();
                }

                bool isVip = await _paymentService.CheckIsVipAsync(userId);

                return Ok(new { isVip = isVip });
            }
            catch
            {
                return StatusCode(500, new { isVip = false });
            }
        }
    }
}