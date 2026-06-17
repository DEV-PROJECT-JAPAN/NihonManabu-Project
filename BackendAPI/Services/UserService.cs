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
            if (httpContext?.User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Chưa đăng nhập");
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : throw new UnauthorizedAccessException();
        }
    }
}