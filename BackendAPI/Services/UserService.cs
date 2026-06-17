using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BackendAPI.Interfaces;

namespace BackendAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            // Nếu không có context hoặc user chưa đăng nhập/chưa xác thực
            if (httpContext?.User.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập hoặc Token không hợp lệ.");
            }

            // Lấy UserId từ Claims của JWT Token dựa trên NameIdentifier
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Không tìm thấy ID người dùng hợp lệ trong Token.");
        }
    }
}