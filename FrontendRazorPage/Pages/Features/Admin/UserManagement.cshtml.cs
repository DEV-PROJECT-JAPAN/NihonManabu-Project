using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Admin
{
    public class UserManagementModel : PageModel
    {
        private readonly UService _userService;

        public UserManagementModel(UService userService)
        {
            _userService = userService;
        }

        public List<UserDto> Users { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }








        public async Task OnGetAsync()
        {
            await LoadUsers();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int userId, string newRole)
        {
            var result = await _userService.ChangeUserRoleAsync(userId, newRole);

            if (result)
            {
                IsSuccess = true;
                Message = $"Đã thay đổi quyền thành công!";
            }
            else
            {
                IsSuccess = false;
                Message = "Có lỗi xảy ra khi thay đổi quyền. Vui lòng thử lại.";
            }

            await LoadUsers();
            return Page();
        }

        private async Task LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            if (users != null)
            {
                Users = users;
            }
            else
            {
                Message = "Không thể tải danh sách người dùng. Vui lòng đăng nhập lại.";
            }
        }
    }
}
