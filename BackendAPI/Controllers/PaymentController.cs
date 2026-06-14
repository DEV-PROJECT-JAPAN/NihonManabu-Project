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

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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
    }
}