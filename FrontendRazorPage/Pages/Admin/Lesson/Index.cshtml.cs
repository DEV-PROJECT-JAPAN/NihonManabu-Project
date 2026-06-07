using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Lesson
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public IndexModel(VocabularyClientService service) => _service = service;

        // Danh sách dùng cho Bộ lọc (Dropdown)
        public List<LevelModel> Levels { get; set; } = new();

        // Danh sách Bài học đổ ra bảng
        public List<LessonModel> Lessons { get; set; } = new();

        // Biến hứng ID từ thanh địa chỉ URL (ví dụ: ?levelId=1)
        [BindProperty(SupportsGet = true)]
        public int? LevelId { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Luôn luôn lấy danh sách Cấp độ về cho Dropdown
            Levels = await _service.GetLevelsAsync();

            if (Levels != null && Levels.Any())
            {
                // 2. Nếu Admin chưa chọn Level nào (mới mở trang), tự động chọn Level đầu tiên làm mặc định
                LevelId ??= Levels.First().Id;

                // 3. Gọi API lấy bài học theo đúng Level đang được chọn
                Lessons = await _service.GetLessonsAsync(LevelId.Value);
            }
        }

        // Hứng sự kiện bấm nút Xóa
        public async Task<IActionResult> OnPostDeleteAsync(int id, int currentLevelId)
        {
            bool success = await _service.DeleteLessonAsync(id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Xóa Bài học thất bại. Có thể bài học này đang chứa Từ vựng bên trong!");
                Levels = await _service.GetLevelsAsync();
                Lessons = await _service.GetLessonsAsync(currentLevelId);
                return Page();
            }

            // Xóa thành công thì load lại trang và GIỮ NGUYÊN bộ lọc Level hiện tại
            return RedirectToPage("/Admin/Lesson/Index", new { levelId = currentLevelId });
        }
    }
}