using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // Thêm dòng này để bảo vệ trang
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims; // Thêm dòng này để đọc Claims

namespace FrontendRazorPage.Pages.Features.Auth
{
    [Authorize] // Chỉ những người đã đăng nhập mới được vào
    public class ProfileModel : PageModel
    {
        private readonly AuthService _authService;

        public ProfileModel(AuthService authService)
        {
            _authService = authService;
        }

        public ProfileViewModel UserInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Gọi hàm không cần truyền tham số vì đã lấy token từ Cookie trong Service
            UserInfo = await _authService.GetProfileAsync();

            if (UserInfo == null)
            {
                return RedirectToPage("/Features/Auth/Login");
            }

            return Page();
        }
    }
}