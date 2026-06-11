using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BackendAPI.Interfaces;

namespace BackendAPI.Services
{
    public class MockUserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MockUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            return 1; // Trả về ID người dùng cố định (ví dụ: 1) để phục vụ mục đích test
        }
        // get email của user
        public string GetCurrentUsersEmail()
        {
            return "hoangnguyen200525@gmail.com"; // Trả về email người dùng cố định (ví dụ: mockuser@example.com) để phục vụ mục đích test
        }
    }
}