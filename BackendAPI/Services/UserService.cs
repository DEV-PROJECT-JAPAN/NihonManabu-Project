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
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {

                // =========================================================================
                // LÀM MẸO BÂY GIỜ (CHƯA CÓ LOGIN)
                // Nếu bạn làm Đăng nhập chưa xong, hệ thống chưa có Token xác thực hợp lệ.
                // Hàm này tự động trả về ID = 1 để các thành viên khác thoải mái test database.
                // =========================================================================
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
            }

            // =========================================================================
            // KHI BẠN LÀM ĐĂNG NHẬP HOÀN THÀNH:
            // Bạn ấy chỉ cần mở file này ra và viết logic bóc tách JWT Token ở đây.
            // Các thành viên khác hoàn toàn KHÔNG BỊ ẢNH HƯỞNG hay phải sửa lại code.
            // =========================================================================
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new Exception("Token không chứa thông tin User ID.");
            }
            return int.TryParse(userIdClaim, out int realUserId) ? realUserId : 1;
        }
    }
}