using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.Lessons
{
    public class CreateModel : PageModel
    {
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService; // Cần dùng để lấy danh sách Level chọn lựa

        public CreateModel(LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        // Đối tượng hứng dữ liệu từ Form điền của Admin
        [BindProperty]
        public LessonAdminModel LessonInput { get; set; } = new();

        // Danh sách Level hiển thị ở thẻ <select> cho Admin chọn
        public List<LevelModel> Levels { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Nạp danh sách Level (N1 -> N5) lên Form khi vừa mở trang
            Levels = await _levelClientService.GetLevelsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra tính hợp lệ của dữ liệu (Validate Form)
            if (!ModelState.IsValid)
            {
                // Nếu điền thiếu hoặc sai, nạp lại danh sách Level và giữ Admin ở lại trang Form
                Levels = await _levelClientService.GetLevelsAsync();
                return Page();
            }

            // Gọi Service đẩy dữ liệu mới tạo xuống Backend API
            bool isSuccess = await _lessonClientService.CreateLessonAsync(LessonInput);

            if (isSuccess)
            {
                Message = "Thêm bài học mới thành công!";
                return RedirectToPage("./Index"); // Thành công thì quay về trang danh sách
            }

            // Thất bại (Lỗi API/Database)
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình lưu dữ liệu vào hệ thống.");
            Levels = await _levelClientService.GetLevelsAsync();
            return Page();
        }
    }
}