using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Core.Services; // Gọi service lấy danh mục
using FrontendRazorPage.Models.AdminModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class CreateModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService; // Thêm service để lấy danh mục bài học

        public CreateModel(GrammarClientService grammarService, LessonClientService lessonClientService, LevelClientService levelClientService )
        {
            _grammarService = grammarService;
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        [BindProperty]
        public GrammarAdminModel Grammar { get; set; } = new();

        // Thuộc tính phục vụ trạng thái chọn lọc trên Form
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }

        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        // Nhận tham số lessonId từ trang Index truyền sang (nếu có)
        public async Task OnGetAsync(int? lessonId = null)
        {
            // 1. Luôn nạp toàn bộ cấp độ hệ thống
            Levels = await _levelClientService.GetLevelsAsync() ?? new();

            if (lessonId.HasValue && lessonId.Value > 0)
            {
                // Nếu đi từ trang Index đã chọn sẵn bài học
                Grammar.LessonId = lessonId.Value;

                // Tìm ngược lại xem bài học này thuộc Level nào để hiển thị select box cho chuẩn
                // (Khôi có thể gọi API hoặc để JavaScript xử lý, ở đây ta nạp bài học theo level nếu chọn thủ công)
            }

            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Xóa validation của các trường không nhập từ Form nếu ModelState bắt bẻ
            ModelState.Remove("Grammar.CreatedAt");
            ModelState.Remove("Grammar.UpdatedAt");

            if (!ModelState.IsValid)
            {
                // Nạp lại danh mục nếu form lỗi để tránh rỗng dropdown
                Levels = await _levelClientService.GetLevelsAsync() ?? new();
                if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
                {
                    Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value) ?? new();
                }
                return Page();
            }

            var success = await _grammarService.CreateAsync(Grammar);
            if (success)
            {
                // Lưu thành công quay về đúng trang danh sách kèm bộ lọc bài học vừa thêm
                return RedirectToPage("./Index");
                //new { levelId = SelectedLevelId, lessonId = Grammar.LessonId })
            }

            ModelState.AddModelError(string.Empty, "Lỗi khi thêm mới dữ liệu vào hệ thống.");

            // Nạp lại danh mục
            Levels = await _levelClientService.GetLevelsAsync() ?? new();
            return Page();
        }
    }
}