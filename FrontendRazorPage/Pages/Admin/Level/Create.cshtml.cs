using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Level
{
    public class CreateModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public CreateModel(VocabularyClientService service) => _service = service;

        [BindProperty]
        public LevelModel NewLevel { get; set; } = new();

        public void OnGet()
        {
            // Chỉ cần load form trống, không cần làm gì thêm
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xem form có bị bỏ trống hay vi phạm quy tắc nhập liệu không
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Gọi API Thêm mới
            bool success = await _service.CreateLevelAsync(NewLevel);

            if (success)
            {
                // Thành công thì đá về trang danh sách
                return RedirectToPage("/Admin/Level/Index");
            }

            // Nếu Backend báo lỗi (ví dụ trùng tên cấp độ), báo lỗi ra màn hình
            ModelState.AddModelError(string.Empty, "Thêm mới thất bại. Vui lòng kiểm tra lại kết nối hoặc dữ liệu.");
            return Page();
        }
    }
}