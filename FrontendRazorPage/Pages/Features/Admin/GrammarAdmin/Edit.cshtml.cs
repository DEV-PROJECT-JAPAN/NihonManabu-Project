using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Core.Services; // Gọi service danh mục
using FrontendRazorPage.Models.AdminModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class EditModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService; // Tiêm thêm service lấy bài học

        public EditModel(GrammarClientService grammarService, LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _grammarService = grammarService;
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        [BindProperty]
        public GrammarAdminModel Grammar { get; set; } = new();

        // Trạng thái chọn cấp độ trên giao diện Edit
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }

        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // 1. Lấy dữ liệu thực thể gốc cần chỉnh sửa từ API về
            Grammar = await _grammarService.GetByIdForAdminAsync(id);
            if (Grammar == null)
            {
                return RedirectToPage("./Index");
            }

            // 2. Luôn nạp danh sách Cấp độ cho Dropdown số 1
            Levels = await _levelClientService.GetLevelsAsync() ?? new();

            // 3. Nếu Admin chủ động bấm chọn đổi Cấp độ trên giao diện
            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value) ?? new();
            }
            else
            {
                // Nếu mới vào trang lần đầu, hệ thống tự động bốc ngược danh sách bài học dựa theo LessonId gốc của bản ghi
                // Khôi bọc lót đoạn này để form hiển thị sẵn bài học cũ của bản ghi đó nhé
                // Giả định nạp tạm bài học (Hoặc nếu hệ thống có API lấy Level từ LessonId thì gọi vào đây)
                // Để an toàn, ta cứ cho load sẵn bài học nếu Admin chọn cấp độ
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                // Nạp lại danh mục nếu form lỗi tránh rỗng select box
                Levels = await _levelClientService.GetLevelsAsync() ?? new();
                if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
                {
                    Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value) ?? new();
                }
                return Page();
            }

            var success = await _grammarService.UpdateAsync(id, Grammar);
            if (success)
            {
                // Cập nhật thành công, quay về trang danh sách kèm bộ lọc để Admin đỡ phải chọn lại
                return RedirectToPage("./Index", new { levelId = SelectedLevelId, lessonId = Grammar.LessonId });
            }

            ModelState.AddModelError(string.Empty, "Lỗi trong quá trình cập nhật dữ liệu vào hệ thống.");
            Levels = await _levelClientService.GetLevelsAsync() ?? new();
            return Page();
        }
    }
}