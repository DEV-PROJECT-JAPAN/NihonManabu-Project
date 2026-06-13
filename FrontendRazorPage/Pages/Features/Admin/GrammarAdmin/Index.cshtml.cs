using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class IndexModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;
        // Tái sử dụng service lấy danh mục cấp độ/bài học

        // Sử dụng thuộc tính ràng buộc dữ liệu BindProperty giống trang Từ Vựng
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedLessonId { get; set; }

        // Danh sách đổ vào các thanh bộ lọc Dropdown
        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        // Danh sách hiển thị lên bảng dữ liệu chính
        public List<GrammarAdminModel> Grammars { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public IndexModel(GrammarClientService grammarService, LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _grammarService = grammarService;
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        public async Task OnGetAsync()
        {
            // Bước 1: Luôn nạp danh sách Cấp độ (N5, N4...) cho Dropdown số 1
            Levels = await _levelClientService.GetLevelsAsync() ?? new();

            // Bước 2: Nếu Admin đã chọn một Cấp độ cụ thể -> Load các Bài học tương ứng của cấp độ đó
            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value);
            }

            // Bước 3: Logic lấy dữ liệu: Chỉ lôi ngữ pháp ra khi Admin đã chọn đích danh một Bài học cụ thể
            if (SelectedLessonId.HasValue && SelectedLessonId.Value > 0)
            {
                var allGrammars = await _grammarService.GetAllForAdminAsync() ?? new List<GrammarAdminModel>();

                // Lọc chính xác theo mã bài học
                Grammars = allGrammars.Where(g => g.LessonId == SelectedLessonId.Value).ToList();
            }
            else
            {
                // Nếu chưa chọn Bài học -> Để bảng trống, hướng dẫn Admin chọn bài học giống hệt bên Từ Vựng
                Grammars = new List<GrammarAdminModel>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _grammarService.DeleteAsync(id);
            Message = success ? "Xóa mẫu ngữ pháp thành công!" : "Xóa mẫu ngữ pháp thất bại.";

            // Reload lại trang và giữ nguyên bộ lọc cũ để Admin không phải chọn lại từ đầu
            return RedirectToPage(new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
        }
    }
}