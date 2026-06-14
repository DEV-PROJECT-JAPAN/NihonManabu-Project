using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

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

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return BadRequest(new { Success = false, Message = "Email không hợp lệ." });

            string otpCode = new Random().Next(100000, 999999).ToString();
            await _emailService.SendEmailAsync(request.Email, "Mã xác thực OTP", $"Mã OTP của bạn là: {otpCode}");
            _cache.Set(request.Email, otpCode, TimeSpan.FromMinutes(5));

            return Ok(new { Success = true, Message = "Mã OTP đã được gửi." });
        }

        [HttpPost("verify-and-register")]
        public async Task<IActionResult> VerifyAndRegister(
     [FromBody] RegisterDto data)
        {
            if (!_cache.TryGetValue(data.Email, out string? savedOtp) ||
                savedOtp != data.OtpCode)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "OTP không đúng hoặc đã hết hạn."
                });
            }

            if (data.Password != data.ConfirmPassword)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Mật khẩu xác nhận không khớp."
                });
            }

            var existingUser =
                await _authRepository.GetUserByEmailAsync(data.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email đã được sử dụng."
                });
            }

            _cache.Remove(data.Email);

            var newUser = new User
            {
                Email = data.Email,
                UserName = data.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.Password),
                IsEmailConfirmed = true,
                Role = "User",
                CurrentStreak = 0,
                TotalExp = 0,
                LastActiveDate = DateTime.UtcNow
            };

            await _authRepository.CreateUserAsync(newUser);

            return Ok(new
            {
                Success = true,
                Message = "Đăng ký thành công!"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto data)
        {
            var user = await _authRepository.GetUserByEmailAsync(data.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash))
                return Unauthorized(new { Success = false, Message = "Sai email hoặc mật khẩu." });

            // Logic Streak
            var today = DateTime.UtcNow.Date;
            if (user.LastActiveDate == null || user.LastActiveDate.Value.Date != today)
            {
                if (user.LastActiveDate != null && user.LastActiveDate.Value.Date == today.AddDays(-1))
                    user.CurrentStreak += 1;
                else
                    user.CurrentStreak = 1;

                user.LastActiveDate = today;
                await _authRepository.UpdateUserAsync(user);
            }

            return Ok(new
            {
                Success = true,
                Message = "Đăng nhập thành công!",
                Token = _tokenService.GenerateJwtToken(user),
                Role = user.Role
            });
        }

        [Authorize] // Bắt buộc phải có token mới lấy được profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy email từ claims (phải trùng với ClaimTypes.Email trong TokenService)
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized(new { Success = false, Message = "Token không hợp lệ." });

            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound(new { Success = false, Message = "Người dùng không tồn tại." });

            // Trả về DTO thay vì Object ẩn danh để frontend dễ binding
            return Ok(new
            {
                Success = true,
                Data = new
                {
                    FullName = user.UserName,
                    user.Email,
                    user.TotalExp,
                    user.CurrentStreak
                }
            });
        }
    }
}