using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models; // Đảm bảo chứa class LessonAdminModel tương ứng cấu hình bảng
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.Lessons
{
    public class IndexModel : PageModel
    {
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService; // ➕ Thêm Service Level để lấy danh sách lọc

        public IndexModel(LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        public List<LessonAdminModel> Lessons { get; set; } = new();
        public List<LevelModel> Levels { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }
        [TempData]
        public string Message { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // 1. Luôn nạp danh sách Level lên bộ lọc Dropdown
            Levels = await _levelClientService.GetLevelsAsync();

            // 2. Kiểm tra nếu Admin có chọn lọc theo LevelId cụ thể
            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                // 🟢 Gọi thẳng hàm lọc của Admin để lấy nguyên cục dữ liệu có sẵn ngày tháng từ Backend
                Lessons = await _lessonClientService.GetLessonsByLevelForAdminAsync(SelectedLevelId.Value);
            }
            else
            {
                // Nếu không chọn lọc hoặc chọn "Tất cả bài học" -> Lấy toàn bộ như cũ
                Lessons = await _lessonClientService.GetLessonsForAdminAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _lessonClientService.DeleteLessonAsync(id);
            Message = success ? "Xóa bài học thành công!" : "Xóa bài học thất bại.";

            return RedirectToPage();
        }
    }
}