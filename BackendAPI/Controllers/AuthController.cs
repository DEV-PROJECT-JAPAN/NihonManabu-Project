using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthRepository authRepository, IEmailService emailService, IMemoryCache cache, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _emailService = emailService;
            _cache = cache;
            _tokenService = tokenService;
        }

        // 1. API: send-otp
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email không hợp lệ.");

            try
            {
                string otpCode = new Random().Next(100000, 999999).ToString();
                string emailSubject = "Mã xác thực OTP Đăng ký tài khoản";
                string emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee;'>
                <h2 style='color: #007bff; text-align: center;'>XÁC THỰC TÀI KHOẢN</h2>
                <p>Xin chào,</p>
                <p>Bạn đang thực hiện đăng ký tài khoản tại hệ thống của chúng tôi. Mã OTP xác thực của bạn là:</p>
                <div style='background: #f4f4f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #333; margin: 20px 0;'>
                    {otpCode}
                </div>
                <p style='color: #666; font-size: 12px;'>Mã này có hiệu lực trong vòng 5 phút. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
            </div>";

                await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                _cache.Set(email, otpCode, TimeSpan.FromMinutes(5));

                return Ok("Mã OTP đã được gửi đến Gmail của bạn. Hãy kiểm tra hòm thư!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi gửi mail: {ex.Message}");
            }
        }

        // 2. API: verify-and-register
        [HttpPost("verify-and-register")]
        public async Task<IActionResult> VerifyAndRegister([FromQuery] string otp, [FromBody] RegisterDto data)
        {
            if (data == null || string.IsNullOrEmpty(otp)) return BadRequest("Dữ liệu đăng ký không hợp lệ.");

            try
            {
                if (_cache.TryGetValue(data.Email, out string? savedOtp))
                {
                    if (savedOtp == otp)
                    {
                        _cache.Remove(data.Email);

                        var newUser = new User
                        {
                            Email = data.Email,
                            UserName = data.Email,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.Password),
                            IsEmailConfirmed = true,
                            Role = "User",
                            TotalExp = 0,
                            CurrentStreak = 0
                        };

                        var createdUser = await _authRepository.CreateUserAsync(newUser);

                        if (createdUser != null)
                        {
                            return Ok(new { Success = true, Message = "Xác thực OTP thành công. Tài khoản của bạn đã được tạo!" });
                        }
                        else
                        {
                            return StatusCode(500, "Lỗi hệ thống: Không thể lưu thông tin tài khoản xuống cơ sở dữ liệu.");
                        }
                    }
                    else
                    {
                        return BadRequest("Mã OTP nhập vào không chính xác. Vui lòng kiểm tra lại.");
                    }
                }
                return BadRequest("Mã OTP đã hết hạn hoặc không tồn tại. Vui lòng bấm gửi lại mã mới.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xử lý hệ thống: {ex.Message}");
            }
        }

        // 3. API: login (Đã khôi phục đầy đủ logic Streak)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto data)
        {
            var user = await _authRepository.GetUserByEmailAsync(data.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash))
            {
                return BadRequest(new { Success = false, Message = "Tài khoản hoặc mật khẩu không chính xác." });
            }

            // --- KHỐI LOGIC TÍNH TOÁN STREAK ĐIỂM DANH ---
            var today = DateTime.UtcNow.Date;

            if (user.LastActiveDate == null)
            {
                user.CurrentStreak = 1;
                user.LastActiveDate = today;
            }
            else
            {
                var yesterday = today.AddDays(-1);
                var lastActive = user.LastActiveDate.Value.Date;

                if (lastActive == today)
                {
                    /* Đã hoạt động hôm nay rồi, giữ nguyên streak */
                }
                else if (lastActive == yesterday)
                {
                    user.CurrentStreak += 1;
                    user.LastActiveDate = today;
                }
                else
                {
                    user.CurrentStreak = 1;
                    user.LastActiveDate = today;
                }
            }

            await _authRepository.UpdateUserAsync(user);

            // 🎯 Tạo chuỗi JWT Token thông qua TokenService đã đăng ký trong Program.cs
            string token = _tokenService.GenerateJwtToken(user);

            // Trả về object có thuộc tính viết hoa chữ đầu chuẩn .NET PascalCase
            return Ok(new
            {
                Success = true,
                Message = "Đăng nhập thành công!",
                Token = token
            });
        }

        [HttpGet("profile/{email}")]
        public async Task<IActionResult> GetProfile(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalExp = user.TotalExp,
                CurrentStreak = user.CurrentStreak,
                Role = user.Role
            });
        }
    }
}