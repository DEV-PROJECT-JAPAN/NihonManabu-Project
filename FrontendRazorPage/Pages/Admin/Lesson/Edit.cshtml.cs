using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Lesson
{
    public class EditModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public EditModel(VocabularyClientService service) => _service = service;

        [BindProperty]
        public LessonModel LessonData { get; set; } = new();

        // Biến chứa danh sách Cấp độ cho thẻ <select>
        public List<LevelModel> Levels { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // 1. Luôn phải nạp danh sách Level trước để rải ra Dropdown
            Levels = await _service.GetLevelsAsync();

            // 2. Lấy dữ liệu bài học cũ theo ID
            var data = await _service.GetLessonByIdAsync(id);

            if (data == null)
            {
                return RedirectToPage("/Admin/Lesson/Index");
            }

            // Đổ dữ liệu cũ vào biến để Binding lên HTML
            LessonData = data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Levels = await _service.GetLevelsAsync(); // Lỗi thì cũng phải nạp lại Dropdown
                return Page();
            }

            bool success = await _service.UpdateLessonAsync(LessonData);

            if (success)
            {
                // Mẹo UX/UI: Sửa xong đá về trang Index, nhưng truyền lại đúng cái LevelId 
                // để giữ nguyên bộ lọc, không làm Admin khó chịu.
                return RedirectToPage("/Admin/Lesson/Index", new { levelId = LessonData.LevelId });
            }

            ModelState.AddModelError(string.Empty, "Cap nhat Bai hoc that bai.");
            Levels = await _service.GetLevelsAsync();
            return Page();
        }
    }
}